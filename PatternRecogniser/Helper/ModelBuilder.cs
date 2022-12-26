using PatternRecogniser.Models;
using System;
using System.IO;
using Tensorflow.Keras.Engine;
using static Tensorflow.Binding;
using static Tensorflow.KerasApi;
using System.Drawing.Imaging;
using Tensorflow;
using System.Collections.Generic;

namespace PatternRecogniser.Helper
{

    public static class ModelBuilder
    {
        public static void Load_Weights(SavedLayerData[] layers, Model neural_net, string modelId) 
        {
            for (int i = 0; i < layers.Length; i++)
            {
                // warstwa
                for (int j = 0; j < layers[i].vars.Length; j++)
                {
                    // zmienna
                    Tensor tensor;
                    if (layers[i].vars[j].shape.Length == 1)
                    {
                        // bias; jeden wymiar, czyli jedna lista
                        tensor = ops.convert_to_tensor (layers[i].vars[j].values.ToArray(), TF_DataType.TF_FLOAT);
                    }
                    else
                    {
                        // kernel; powinno być shape.length == 2
                        var list = new List<float[]> (); // wartości zmiennej
                        for (int k = 0; k < layers[i].vars[j].shape[0]; k++)
                        {
                            float[] arr = new float[layers[i].vars[j].shape[1]];
                            for (int l = 0; l < layers[i].vars[j].shape[1]; l++)
                            {
                                arr[l] = layers[i].vars[j].values[k * (int)layers[i].vars[j].shape[1] + l];
                            }
                            list.Add (arr);
                        }
                        tensor = ops.convert_to_tensor (list.ToArray (), TF_DataType.TF_FLOAT);
                    }

                    var variable = tf.Variable (tensor, name: layers[i].vars[j].name);
                    neural_net.Layers[i].trainable_weights.Add (variable);
                }
                
            }
        }

        public static SavedLayerData[] SerializeModel(Model neural_net, string modelId)
        {
            SavedLayerData[] savedLayers = new SavedLayerData[neural_net.Layers.Count];
            int i = 0;
            foreach(var layer in neural_net.Layers)
            {
                savedLayers[i].vars = new SavedVariableData[layer.trainable_variables.Count];
                var trainable = layer.trainable_variables;
                int j = 0;
                foreach (var variable in trainable)
                {
                    savedLayers[i].vars[j].name = variable.Name.Substring(0, variable.Name.IndexOf(":"));
                    savedLayers[i].vars[j].shape = variable.shape.dims;
                    savedLayers[i].vars[j].values = new List<float> ();
                    foreach (var np in variable.numpy())
                    {
                        var arr = np.ToArray<float> ();
                        savedLayers[i].vars[j].values.AddRange (arr);
                    }
                    j++;
                }
                i++;
            }


            return savedLayers;
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
