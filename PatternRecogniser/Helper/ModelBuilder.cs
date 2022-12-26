using PatternRecogniser.Models;
using System;
using System.IO;
using Tensorflow.Keras.Engine;
using static Tensorflow.Binding;
using static Tensorflow.KerasApi;
using System.Drawing.Imaging;

namespace PatternRecogniser.Helper
{

    public static class ModelBuilder
    {
        const string path = "modelDumyBytes.h5";

        public static void Load_Weights(byte[] bytes, Model neural_net, string modelId) 
        {
            string filename = $"{modelId}.h5";
            File.WriteAllBytes(filename, bytes);
            neural_net.load_weights(filename);
            File.Delete(filename);
        }

        public static byte[] SerializeModel(Model neural_net, string modelId)
        {
            string filename = $"{modelId}.h5";
            neural_net.save_weights(filename);
            byte[] file;
            using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new BinaryReader(stream))
                {
                    file = reader.ReadBytes((int)stream.Length);
                }
            }
            File.Delete(filename);
            return file;
        }

        public static Model CreateModel(int num_classes)
        {
            return  new NeuralNet(new NeuralNetArgs
            {
                NumClasses = num_classes,
                NeuronOfHidden1 = 128,
                Activation1 = keras.activations.Relu,
                NeuronOfHidden2 = 256,
                Activation2 = keras.activations.Relu
            });
        }

    }
}
