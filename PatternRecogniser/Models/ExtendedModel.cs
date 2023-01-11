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
using PatternRecogniser.Messages.Model;

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

    [Index (nameof (userLogin), nameof (name), IsUnique = true)]
    public class ExtendedModel
    {
        [Key]
        public int extendedModelId { get; set; }
        [Required]
        [ForeignKey ("User")]
        public string userLogin { get; set; }
        public string name { get; set; }
        public DistributionType distribution { get; set; }
        public byte[] modelInBytes { get; set; }
        public int num_classes { get; set; }

        public virtual User user { get; set; }
        public virtual ICollection<Pattern> patterns { get; set; }
        public virtual ModelTrainingExperiment modelTrainingExperiment { get; set; } // statistics w diagramie klas
        public virtual ICollection<Experiment> experiments { get; set; }
        private ExtendedModelStringMessages _messages = new ExtendedModelStringMessages ();
        private CancellationToken _cancellationToken; // nie wiem jak wykorzystać IsCancellationRequested więc wszędzie wyrzucam błąd _cancellationToken.ThrowIfCancellationRequested(); 


        public void TrainModel (DistributionType distribution, ITrainingUpdate trainingUpdate, byte[] trainingSet, int trainingPercent, int setsNumber, CancellationToken cancellationToken) // nie potrzebne CancellationToken w późniejszym programie
        {
            try
            {
                _cancellationToken = cancellationToken;
                trainingUpdate.Update (_messages.startTraining + "\n");
                var examplePictures = new Dictionary<string, byte[]> ();
                PatternData patternData = OpenZip (trainingSet, examplePictures);

                if (patternData.IsEmpty ())
                {
                    throw new Exception (_messages.incorectFileStructure);
                }

                this.patterns = new List<Pattern> ();
                foreach (var pair in examplePictures)
                {
                    _cancellationToken.ThrowIfCancellationRequested ();
                    this.patterns.Add (new Pattern (pair.Key, pair.Value));
                }

                switch (distribution)
                {
                    case DistributionType.TrainTest:
                        TrainModelTrainTest (patternData, trainingPercent, 100 - trainingPercent, trainingUpdate); // parameters - zawiera 1 lub 2 liczby, domyślne lub ustawione przez użytkownika
                        break;
                    case DistributionType.CrossValidation:
                        TrainModelCrossValidation (patternData, setsNumber, trainingUpdate);
                        break;
                }
            }
            catch (OperationCanceledException oce)
            {

            }
            catch (Exception e)
            {
                trainingUpdate.Update (e.Message);
            }
        }

        public void TrainModelTrainTest (PatternData data, int train, int test, ITrainingUpdate trainingUpdate)
        {
            _cancellationToken.ThrowIfCancellationRequested ();

            // test = 100 - train;
            // randomise patterns in data
            foreach (List<Pattern> list in data.patterns)
            {
                _cancellationToken.ThrowIfCancellationRequested ();
                Random rng = new Random (); // https://stackoverflow.com/questions/273313/randomize-a-listt
                int n = list.Count;
                while (n > 1)
                {
                    _cancellationToken.ThrowIfCancellationRequested ();
                    n--;
                    int k = rng.Next (n + 1);
                    Pattern value = list[k];
                    list[k] = list[n];
                    list[n] = value;
                }
            }

            PatternData trainData = new PatternData ();
            PatternData testData = new PatternData ();

            foreach (List<Pattern> list in data.patterns)
            {
                _cancellationToken.ThrowIfCancellationRequested ();
                int trainSize = (int)(list.Count * (train / 100.0));
                List<Pattern> trainList = list.GetRange (0, trainSize);
                trainData.AddPatterns (trainList);
                int testSize = (int)(list.Count * (test / 100.0));
                testData.AddPatterns (list.GetRange (trainSize, testSize));
            }

            (ModelTrainingExperiment statistics, Model model) = TrainIndividualModel (trainData, testData, trainingUpdate);
            this.modelInBytes = Helper.ModelBuilder.SerializeModel (model);
            this.modelTrainingExperiment = statistics;
            this.modelTrainingExperiment.extendedModel = this;
        }

        public void TrainModelCrossValidation (PatternData data, int k, ITrainingUpdate trainingUpdate)
        {
            _cancellationToken.ThrowIfCancellationRequested ();
            // k - ile podzbiorów
            Model bestModel = null;
            ModelTrainingExperiment bestStatistics = new ModelTrainingExperiment ();

            // sprawdzamy czy możemy podzielić dane na k
            if (k <= 1)
            {
                throw new Exception (_messages.tooSmalSetsNumber);
            }
            foreach (var list in data.patterns)
            {
                _cancellationToken.ThrowIfCancellationRequested ();
                if (list.Count < k)
                {
                    throw new Exception (_messages.tooLargeSetsNumber);
                }
            }

            // randomizujemy kolejność
            foreach (List<Pattern> list in data.patterns)
            {
                Random rng = new Random (); // https://stackoverflow.com/questions/273313/randomize-a-listt
                int n = list.Count;
                _cancellationToken.ThrowIfCancellationRequested ();

                while (n > 1)
                {
                    _cancellationToken.ThrowIfCancellationRequested ();

                    n--;
                    int r = rng.Next (n + 1);
                    Pattern value = list[r];
                    list[r] = list[n];
                    list[n] = value;
                }
            }

            PatternData[] patternDatas = new PatternData[k];
            for (int j = 0; j < k; j++)
            {
                patternDatas[j] = new PatternData ();
            }

            foreach (List<Pattern> list in data.patterns)
            {
                int size = (int)(list.Count / k); // zawsze w dół zaokrągla
                int start = 0;
                int leftovers = Math.Abs (size * k - list.Count); // powinno być mniejsze od k
                int[] sizes = new int[k];
                for (int i = 0; i < k; i++)
                {
                    _cancellationToken.ThrowIfCancellationRequested ();

                    if (i < leftovers)
                    {
                        sizes[i] = size + 1;
                    }
                    else
                    {
                        sizes[i] = size;
                    }
                }

                for (int i = 0; i < k; i++)
                {
                    _cancellationToken.ThrowIfCancellationRequested ();
                    patternDatas[i].AddPatterns (list.GetRange (start, sizes[i]));
                    start += sizes[i];
                }
            }

            for (int j = 0; j < k; j++)
            {
                // patternDatas[j] - test
                trainingUpdate.Update ($"{_messages.crossValidationModelTraining (j)}\n");
                PatternData train = new PatternData ();
                for (int i = 0; i < k; i++)
                {
                    _cancellationToken.ThrowIfCancellationRequested ();
                    if (i != j)
                    {
                        train.AddPatternData (patternDatas[i]);
                    }
                }

                (ModelTrainingExperiment newStatistics, Model newModel) = TrainIndividualModel (train, patternDatas[j], trainingUpdate);
                if (bestModel == null || newStatistics.recall > bestStatistics.recall)
                {
                    bestModel = newModel;
                    bestStatistics = newStatistics;
                }
            }

            this.modelInBytes = Helper.ModelBuilder.SerializeModel (bestModel);
            this.modelTrainingExperiment = bestStatistics;
            this.modelTrainingExperiment.extendedModel = this;
        }

        public List<RecognisedPatterns> RecognisePattern (Bitmap picture)
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
            Model model = Helper.ModelBuilder.CreateModel (num_classes);
            Helper.ModelBuilder.Load_Weights (modelInBytes, model);
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
                        recognisedPattern.probability = (float)rnn;
                        recognisedPattern.pattern = this.patterns.ElementAt (patternId);
                        toReturn.Add (recognisedPattern);
                        patternId++;
                    }
                }
            }

            return toReturn;
        }

        private (ModelTrainingExperiment statistics, Model model) TrainIndividualModel (PatternData train, PatternData test, ITrainingUpdate trainingUpdate)
        {
            /*****************************************************************************
                Copyright 2018 The TensorFlow.NET Authors. All Rights Reserved.
                Licensed under the Apache License, Version 2.0 (the "License");
                you may not use this file except in compliance with the License.
                You may obtain a copy of the License at
                    http://www.apache.org/licenses/LICENSE-2.0
                Unless required by applicable law or agreed to in writing, software
                distributed under the License is distributed on an "AS IS" BASIS,
                WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
                See the License for the specific language governing permissions and
                limitations under the License.
            ******************************************************************************/
            _cancellationToken.ThrowIfCancellationRequested ();
            tf.enable_eager_execution ();

            num_classes = train.GetNumberOfClasses (); // changed
            float learning_rate = 0.05f; // moved from FullyConnectedKeras
            int display_step = 50; // moved from FullyConnectedKeras
            int batch_size = 100; // moved from FullyConnectedKeras
            int training_steps = 1000; // moved from FullyConnectedKeras

            _cancellationToken.ThrowIfCancellationRequested ();
            // matching our data to expected Tensorflow.NET data
            IDatasetV2 train_data;
            Tensor x_test, y_test, x_train, y_train;
            (x_train, y_train) = train.PatternToTensor ();
            (x_test, y_test) = test.PatternToTensor ();
            train_data = tf.data.Dataset.from_tensor_slices (x_train, y_train);
            train_data = train_data.repeat ()
                .shuffle (50000)
                .batch (batch_size)
                .prefetch (1)
                .take (training_steps);

            // Build neural network model.
            var neural_net = Helper.ModelBuilder.CreateModel (num_classes);

            _cancellationToken.ThrowIfCancellationRequested ();
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

            _cancellationToken.ThrowIfCancellationRequested ();
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

                for (int i = 0; i < gradients.Length; i++)
                {
                    if (gradients[i] == null)
                    {
                        gradients[i] = ops.convert_to_tensor (neural_net.trainable_variables[i]);
                    }
                }

                // Update W and b following gradients.
                optimizer.apply_gradients (zip (gradients, neural_net.trainable_variables.Select (x => x as ResourceVariable)));
            };


            // Run training for the given number of steps.
            foreach (var (step, (batch_x, batch_y)) in enumerate (train_data, 1))
            {
                // Run the optimization to update W and b values.
                run_optimization (batch_x, batch_y);

                _cancellationToken.ThrowIfCancellationRequested ();
                if (step % display_step == 0)
                {
                    var pred = neural_net.Apply (batch_x, training: true);
                    var loss = cross_entropy_loss (pred, batch_y);
                    var acc = accuracy (pred, batch_y);

                    trainingUpdate.Update ($"{_messages.trainingProggres (step, training_steps, loss.numpy (), acc.numpy ())}\n");

                    if ((float)acc >= 0.9)
                    {
                        break;
                    }
                }
            }

            // Test model on validation set.
            ModelTrainingExperiment statistics;
            {
                trainingUpdate.Update ($"{_messages.startValidation}\n");
                var pred = neural_net.Apply (x_test, training: false);
                statistics = new ModelTrainingExperiment (pred, y_test, num_classes);
            }
            return (statistics, neural_net);
        }


        // Obsługa zipów i Bitmap

        private PatternData OpenZip (byte[] bytes, Dictionary<string, byte[]> examplePictuers)
        {
            _cancellationToken.ThrowIfCancellationRequested ();
            PatternData data = new PatternData ();
            if (CheckZipStructure (bytes) == false)
                return data; // zwraca puste dane, potem użytkownikowi mówimy że coś nie halo

            Stream stream = new MemoryStream (bytes);
            data.patterns = new List<List<Pattern>> ();
            using (ZipArchive zip = new ZipArchive (stream))
            {
                foreach (ZipArchiveEntry entry in zip.Entries)
                {
                    _cancellationToken.ThrowIfCancellationRequested ();
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

                        // zapisuje pierwsze zdjęcia z każdego folderu
                        if (!examplePictuers.ContainsKey (patternName))
                        {
                            examplePictuers.Add (patternName, array);
                        }

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

        private bool CheckZipStructure (byte[] bytes)
        {
            // sprawdź czy to w ogóle zip (albo tylko zip będziemy wyświetlać, idk)
            //FileInfo fi = new FileInfo (path);
            //if (!fi.Extension.Equals (".zip"))
            //return false;

            Stream data = new MemoryStream (bytes);
            // otwieramy zip i sprawdzamy strukturę
            using (ZipArchive zip = new ZipArchive (data))
            {
                foreach (ZipArchiveEntry entry in zip.Entries)
                {
                    _cancellationToken.ThrowIfCancellationRequested ();
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
                    _cancellationToken.ThrowIfCancellationRequested ();
                    // sprowadzamy kolor do skali szarości
                    Color pixelColor = contrastBmp.GetPixel (i, j);
                    int avg = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;

                    // odnaleziony kolor sprowadzamy do wartości 0/1
                    // normalnie 0 - czarny, 255 - biały
                    // zatem dzielimy przez 255 i zamieniamy
                    // wpisujemy do pola [j, i] żeby zapobiec obróceniu obrazka
                    float tmp = (float)avg / (float)255;
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
