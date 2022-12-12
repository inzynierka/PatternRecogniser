using Microsoft.EntityFrameworkCore;
using PatternRecogniser.ThreadsComunication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
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

namespace PatternRecogniser.Models
{
    public enum DistributionType
    {
        TrainTest, CrossValidation
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

        public virtual User user { get; set; }
        public virtual ICollection<Pattern> patterns { get; set; }
        public virtual ModelTrainingExperiment modelTrainingExperiment { get; set; } // statistics w diagramie klas
        public virtual ICollection<Experiment> experiments { get; set; }

        private Model model; 

        // tymczasowo asynchroniczna w celu testowania
        public async void TrainModel(DistributionType distribution, ITrainingUpdate trainingUpdate, CancellationToken stoppingToken, PatternData patternData, List<int> parameters) // nie potrzebne CancellationToken w późniejszym programie
        {

            this.distribution = distribution;
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
                    TrainModelTrainTest(patternData, parameters[0], parameters[1]); // parameters - zawiera 1 lub 2 liczby, domyślne lub ustawione przez użytkownika
                    break;
                case DistributionType.CrossValidation:
                    TrainModelCrossValidation(patternData, parameters[0]);
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
                int trainSize = list.Count * (train / 100);
                List<Pattern> trainList = list.GetRange (0, trainSize);
                trainData.AddPatterns (trainList);
                int testSize = list.Count * (test / 100);
                testData.AddPatterns (list.GetRange (trainSize + 1, testSize));
            }

            TrainIndividualModel (trainData, testData);
        }

        public void TrainModelCrossValidation(PatternData data, int n) 
        { 
            // jeszcze nie zaimplementowane
        }

        public List<RecognisedPatterns> RecognisePattern(Bitmap picture)
        {
            var toRecognise = new List<int[,]> { user.NormaliseData(picture) };
            NDArray nDArray = new NDArray (toRecognise.ToArray());
            var result = model.Apply (nDArray, training: false); // i coś z result odczytujemy

            return new List<RecognisedPatterns>(); // returns a string that follows json formatting
        }

        private void TrainIndividualModel(PatternData train, PatternData test) 
        {
            tf.enable_eager_execution();

            //PrepareData ();
            int num_classes = train.GetNumberOfClasses(); // changed
            float learning_rate = 0.1f; // moved from FullyConnectedKeras
            int display_step = 100; // moved from FullyConnectedKeras
            int batch_size = 256; // moved from FullyConnectedKeras
            int training_steps = 1000; // moved from FullyConnectedKeras

            // matching our data to expected Tensorflow.NET data
            IDatasetV2 train_data;
            NDArray x_test, y_test, x_train, y_train;
            (x_train, y_train) = train.PatternToNDArray();
            (x_test, y_test) = test.PatternToNDArray ();
            train_data = tf.data.Dataset.from_tensor_slices (x_train, y_train);
            train_data = train_data.repeat ()
                .shuffle (5000)
                .batch (batch_size)
                .prefetch (1)
                .take (training_steps);

            // Build neural network model.
            var neural_net = new NeuralNet (new NeuralNetArgs
            {
                NumClasses = num_classes,
                NeuronOfHidden1 = 128,
                Activation1 = keras.activations.Relu,
                NeuronOfHidden2 = 256,
                Activation2 = keras.activations.Relu
            });

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
                    print ($"step: {step}, loss: {(float)loss}, accuracy: {(float)acc}");
                }
            }

            // Test model on validation set.
            {
                var pred = neural_net.Apply (x_test, training: false);
                modelTrainingExperiment.accuracy = (float)accuracy (pred, y_test); // changed
                //print ($"Test Accuracy: {this.accuracy}"); // commented
                // tu jakoś trzeba dopisać wyniki różne do modelTrainingExperiment
            }

            model = neural_net; // added
        }
    }
}
