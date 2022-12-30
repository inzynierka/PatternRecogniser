using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using PatternRecogniser.ThreadsComunication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tensorflow;
using Tensorflow.Keras;
using Tensorflow.Keras.ArgsDefinition;
using Tensorflow.Keras.Engine;
using Tensorflow.NumPy;
using static Tensorflow.Binding;
using static Tensorflow.KerasApi;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Http;
using PatternRecogniser.Helper;

namespace PatternRecogniser.Models
{
    public enum DistributionType
    {
        TrainTest, CrossValidation
    }

    [Serializable]
    public struct SavedVariableData
    {
        public string name;
        public long[] shape; // np. Shape(784,128) będzie zapisany jako {784, 128}
        public List<float> values; // wszystkie wartości zapisane w jednej liście
    }

    [Serializable]
    public struct SavedLayerData
    {
        public SavedVariableData[] vars; // najpewniej 2 variable
    }

    [Index(nameof(userLogin), nameof(name), IsUnique = true)] 
    public class ExtendedModel
    {
        [Key]
        public int extendedModelId { get; set; }
        [Required]
        [ForeignKey("User")]
        public string userLogin { get; set; }
        public string name { get; set; }
        public DistributionType distribution { get; set; }
        public byte[] modelInBytes { get; set; }  // pamiętać, by dodać to do bazy
        public int num_classes { get; set; }

        public virtual User user { get; set; }
        public virtual ICollection<Pattern> patterns { get; set; }
        public virtual ModelTrainingExperiment modelTrainingExperiment { get; set; } // statistics w diagramie klas
        public virtual ICollection<Experiment> experiments { get; set; }

        

        public void TrainModel(DistributionType distribution, ITrainingUpdate trainingUpdate, byte[] trainingSet, int trainingPercent, int setsNumber) // nie potrzebne CancellationToken w późniejszym programie
        {
            trainingUpdate.Update("Rozpoczęto trenowanie");
            //this.distribution = info.distributionType;
            //modelTrainingExperiment = new ModelTrainingExperiment ();
            PatternData patternData = OpenZip (trainingSet);
            
            if (patternData.IsEmpty())
            {
                throw new Exception ("Zła struktura pliku");
            }

            // zapisanie przykładowych patternów
            this.patterns = new List<Pattern> ();
            foreach (List<Pattern> patternList in patternData.patterns)
            {
                this.patterns.Add (patternList[0]);
            }

            // trenowanie 
            /*for (int i = 0; i < 3; i++)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                trainingUpdate.Update($"info dla usera {userLogin}: {DateTime.Now}\n"); // zapisuje info
            }
            // trenowanie

            //validaja
            await Task.Delay(TimeSpan.FromSeconds(3));
            trainingUpdate.Update($"info dla usera {userLogin}: start validacji {DateTime.Now}\n"); // zapisuje info
            var experyment = new ModelTrainingExperiment()
            {
                extendedModel = this
            };
            modelTrainingExperiment = experyment;*/
            //validacja

            switch (distribution)
            {
                case DistributionType.TrainTest:
                    TrainModelTrainTest(patternData, trainingPercent, 100 - trainingPercent); // parameters - zawiera 1 lub 2 liczby, domyślne lub ustawione przez użytkownika
                    break;
                case DistributionType.CrossValidation:
                    TrainModelCrossValidation(patternData, setsNumber);
                    break;
            }
            // parameters może być jedną wartością, bo w sumie train+test muszą dawać 100

            // zapisanie pojedynczych obrazków do patterns
        }

        public void TrainModelTrainTest(PatternData data, int train, int test) 
        {
            // test = 100 - train;
            // randomise patterns in data
            foreach(List<Pattern> list in data.patterns)
            {
                Random rng = new Random (); // https://stackoverflow.com/questions/273313/randomize-a-listt
                int n = list.Count;
                while (n > 1)
                {
                    n--;
                    int k = rng.Next (n + 1);
                    Pattern value = list[k];
                    list[k] = list[n];
                    list[n] = value;
                }
            }

            PatternData trainData = new PatternData ();
            PatternData testData = new PatternData ();

            foreach(List<Pattern> list in data.patterns)
            {
                // potencjalnie indeksy/zakresy do poprawienia
                int trainSize = (int)(list.Count * (train / 100.0));
                List<Pattern> trainList = list.GetRange (0, trainSize);
                trainData.AddPatterns (trainList);
                int testSize = (int)(list.Count * (test / 100.0));
                testData.AddPatterns (list.GetRange (trainSize, testSize));
            }

            TrainIndividualModel (trainData, testData);
        }

        public void TrainModelCrossValidation(PatternData data, int n) 
        { 
            // jeszcze nie zaimplementowane
        }

        public List<RecognisedPatterns> RecognisePattern(Bitmap picture)
        {
            var toReturn = new List<RecognisedPatterns> ();
            var toRecognise = NormaliseData (picture);
            float[] pic = new float[toRecognise.GetLength (0) * toRecognise.GetLength (1)];
            int i = 0;
            foreach (var pixel in toRecognise)
            {
                pic[i] = pixel;
                i++;
            }
            List<float[]> picAsList = new List<float[]> { pic };
            Tensor picTensor = ops.convert_to_tensor (picAsList.ToArray (), TF_DataType.TF_FLOAT);
            Model model = Helper.ModelBuilder.CreateModel(num_classes);
            Helper.ModelBuilder.Load_Weights(modelInBytes, model);
            var result = model.Apply (picTensor, training: false); // i coś z result odczytujemy
            int patternId = 0;
            foreach (var r in result)
            {
                foreach (var rn in r.numpy ())
                {
                    var arr = rn.ToArray ();
                    float prob0 = arr[0];
                    foreach (var rnn in rn.numpy ())
                    {
                        RecognisedPatterns recognisedPattern = new RecognisedPatterns ();
                        recognisedPattern.patternId = patternId;
                        recognisedPattern.probability = rnn;
                        recognisedPattern.pattern = this.patterns.ElementAt(patternId);
                        toReturn.Add (recognisedPattern);
                        patternId++;
                    }
                }
            }

            return toReturn; 
        }

        private void TrainIndividualModel(PatternData train, PatternData test) 
        {
            tf.enable_eager_execution ();

            //PrepareData ();
            num_classes = train.GetNumberOfClasses (); // changed
            float learning_rate = 0.1f; // moved from FullyConnectedKeras
            int display_step = 100; // moved from FullyConnectedKeras
            int batch_size = 256; // moved from FullyConnectedKeras
            int training_steps = 1000; // moved from FullyConnectedKeras

            // matching our data to expected Tensorflow.NET data
            IDatasetV2 train_data;
            Tensor x_test, y_test, x_train, y_train;
            (x_train, y_train) = train.PatternToTensor ();
            (x_test, y_test) = test.PatternToTensor ();
            train_data = tf.data.Dataset.from_tensor_slices (x_train, y_train);
            train_data = train_data.repeat ()
                .shuffle (5000)
                .batch (batch_size)
                .prefetch (1)
                .take (training_steps);

            // Build neural network model.
            var neural_net = Helper.ModelBuilder.CreateModel(num_classes);

            // Cross-Entropy Loss.
            // Note that this will apply 'softmax' to the logits.
            Func<Tensor, Tensor, Tensor> cross_entropy_loss = (x, y) =>
            {
                // Convert labels to int 64 for tf cross-entropy function.
                y = tf.cast (y, tf.int64);
                // Apply softmax to logits and compute cross-entropy.
                var loss = tf.nn.sparse_softmax_cross_entropy_with_logits (labels: y, logits: x);
                // Average loss across the batch.
                return tf.reduce_mean (loss);
            };

            // Accuracy metric.
            Func<Tensor, Tensor, Tensor> accuracy = (y_pred, y_true) =>
            {
                // Predicted class is the index of highest score in prediction vector (i.e. argmax).
                var correct_prediction = tf.equal (tf.math.argmax (y_pred, 1), tf.cast (y_true, tf.int64));
                return tf.reduce_mean (tf.cast (correct_prediction, tf.float32), axis: -1);
            };

            // Stochastic gradient descent optimizer.
            var optimizer = keras.optimizers.SGD (learning_rate);

            // Optimization process.
            Action<Tensor, Tensor> run_optimization = (x, y) =>
            {
                // Wrap computation inside a GradientTape for automatic differentiation.
                using var g = tf.GradientTape ();
                // Forward pass.
                var pred = neural_net.Apply (x, training: true);
                var loss = cross_entropy_loss (pred, y);

                // Compute gradients.
                var gradients = g.gradient (loss, neural_net.trainable_variables);

                // Update W and b following gradients.
                optimizer.apply_gradients (zip (gradients, neural_net.trainable_variables.Select (x => x as ResourceVariable)));
            };


            // Run training for the given number of steps.
            foreach (var (step, (batch_x, batch_y)) in enumerate (train_data, 1))
            {
                // Run the optimization to update W and b values.
                run_optimization (batch_x, batch_y);

                if (step % display_step == 0)
                {
                    var pred = neural_net.Apply (batch_x, training: true);
                    var loss = cross_entropy_loss (pred, batch_y);
                    var acc = accuracy (pred, batch_y);
                    // tu jakoś training update
                }
            }

            // Test model on validation set.
            {
                var pred = neural_net.Apply (x_test, training: false);
                modelTrainingExperiment = new ModelTrainingExperiment (pred, y_test, num_classes);
                //modelTrainingExperiment.accuracy = (float)accuracy (pred, y_test); // changed
            }

            modelInBytes = Helper.ModelBuilder.SerializeModel(neural_net); 
            modelTrainingExperiment.extendedModel = this;
        }


        // Obsługa zipów i Bitmap

        private PatternData OpenZip(byte[] bytes)
        {
            PatternData data = new PatternData();
            if (CheckZipStructure(bytes) == false)
                return data; // zwraca puste dane, potem użytkownikowi mówimy że coś nie halo

            Stream stream = new MemoryStream(bytes);
            data.patterns = new List<List<Pattern>> ();
            using (ZipArchive zip = new ZipArchive(stream))
            {
                foreach (ZipArchiveEntry entry in zip.Entries)
                {
                    string fullName = entry.FullName;
                    string name = entry.Name;
                    if (name.Length > 0) // plik nie folder
                    {
                        // etykieta pattern - nazwa folderu - FullName do /
                        string patternName = fullName.Substring (0, fullName.IndexOf ('/'));

                        // obrazek patternu - byte array zawartości
                        Stream reader = entry.Open ();
                        MemoryStream memstream = new MemoryStream ();
                        reader.CopyTo (memstream);
                        byte[] array = memstream.ToArray ();
                        MemoryStream ms = new MemoryStream (array);
                        Bitmap bmp = new Bitmap (ms);
                        int[,] matrix = NormaliseData (bmp);

                        // stwórz Pattern
                        Pattern pattern = new Pattern (patternName, matrix); // konstruktor zamienia int[,] na byte[]

                        // dodaj do PatternData
                        data.AddPattern (pattern);
                    }
                }
            }

            return data;
        }

        private bool CheckZipStructure(byte[] bytes)
        {
            // sprawdź czy to w ogóle zip (albo tylko zip będziemy wyświetlać, idk)
            //FileInfo fi = new FileInfo (path);
            //if (!fi.Extension.Equals (".zip"))
            //return false;

            Stream data = new MemoryStream(bytes);
            // otwieramy zip i sprawdzamy strukturę
            using (ZipArchive zip = new ZipArchive(data))
            {
                foreach (ZipArchiveEntry entry in zip.Entries)
                {
                    string tmp = entry.FullName;
                    int ind = tmp.IndexOf ('/');

                    // gdyby Substring==0 to byłby to folder
                    if (ind > 0 && tmp.Substring (ind + 1).Length != 0)
                    {
                        // plik w folderze
                        // sprawdzamy czy nie ma podfolderów
                        if (tmp.Substring (ind + 1).IndexOf ('/') >= 0)
                        {
                            return false;
                        }
                    }
                    else if (ind < 0) // plik poza folderem
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public int[,] NormaliseData (Bitmap bmp)
        {
            // jeden obrazek Bitmap
            bmp = new Bitmap (bmp, 28, 28);

            int height = bmp.Height; // 28
            int width = bmp.Width; // 28
            Bitmap contrastBmp = AdjustContrast (bmp, 100.0f);

            int[,] binaryMatrix = new int[height, width];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    // sprowadzamy kolor do skali szarości
                    Color pixelColor = contrastBmp.GetPixel (i, j);
                    int avg = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;

                    // odnaleziony kolor sprowadzamy do wartości 0/1
                    // normalnie 0 - czarny, 255 - biały
                    // zatem dzielimy przez 255 i zamieniamy
                    // wpisujemy do pola [j, i] żeby zapobiec obróceniu obrazka
                    float tmp = avg / 255;
                    if (tmp < 0.5) // czarny
                    {
                        binaryMatrix[j, i] = 1;
                    }
                    else // biały
                    {
                        binaryMatrix[j, i] = 0;
                    }
                }
            }

            return binaryMatrix;
        }

        private static Bitmap AdjustContrast (Bitmap Image, float Value) // https://stackoverflow.com/questions/3115076/adjust-the-contrast-of-an-image-in-c-sharp-efficiently
        {
            Value = (100.0f + Value) / 100.0f;
            Value *= Value;
            Bitmap NewBitmap = (Bitmap)Image.Clone ();
            BitmapData data = NewBitmap.LockBits (
                new Rectangle (0, 0, NewBitmap.Width, NewBitmap.Height),
                ImageLockMode.ReadWrite,
                NewBitmap.PixelFormat);
            int Height = NewBitmap.Height;
            int Width = NewBitmap.Width;

            unsafe // włączone "allow unsafe code"!!!
            {
                for (int y = 0; y < Height; ++y)
                {
                    byte* row = (byte*)data.Scan0 + (y * data.Stride);
                    int columnOffset = 0;
                    for (int x = 0; x < Width; ++x)
                    {
                        byte B = row[columnOffset];
                        byte G = row[columnOffset + 1];
                        byte R = row[columnOffset + 2];

                        float Red = R / 255.0f;
                        float Green = G / 255.0f;
                        float Blue = B / 255.0f;
                        Red = (((Red - 0.5f) * Value) + 0.5f) * 255.0f;
                        Green = (((Green - 0.5f) * Value) + 0.5f) * 255.0f;
                        Blue = (((Blue - 0.5f) * Value) + 0.5f) * 255.0f;

                        int iR = (int)Red;
                        iR = iR > 255 ? 255 : iR;
                        iR = iR < 0 ? 0 : iR;
                        int iG = (int)Green;
                        iG = iG > 255 ? 255 : iG;
                        iG = iG < 0 ? 0 : iG;
                        int iB = (int)Blue;
                        iB = iB > 255 ? 255 : iB;
                        iB = iB < 0 ? 0 : iB;

                        row[columnOffset] = (byte)iB;
                        row[columnOffset + 1] = (byte)iG;
                        row[columnOffset + 2] = (byte)iR;

                        columnOffset += 4;
                    }
                }
            }

            NewBitmap.UnlockBits (data);

            return NewBitmap;
        }
    }
}
