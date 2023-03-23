using PatternRecogniser.Models;
using System;
using System.IO;
using Tensorflow.Keras.Engine;
using static Tensorflow.Binding;
using static Tensorflow.KerasApi;
using System.Drawing.Imaging;
using Tensorflow;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

namespace PatternRecogniser.Helper
{

    public static class ModelBuilder
    {
        public static void Load_Weights(byte[] modelInBytes, Model neural_net) 
        {

            SavedLayerData[] layers;
            using (var ms = new MemoryStream(modelInBytes))
            {
                var bf = new BinaryFormatter();
                layers = (SavedLayerData[])bf.Deserialize(ms);
            }

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

        public static byte[] SerializeModel(Model neural_net)
        {
            SavedLayerData[] savedLayers = new SavedLayerData[neural_net.Layers.Count];
            int i = 0;
            foreach (var layer in neural_net.Layers)
            {
                savedLayers[i].vars = new SavedVariableData[layer.trainable_variables.Count];
                var trainable = layer.trainable_variables;
                int j = 0;
                foreach (var variable in trainable)
                {
                    savedLayers[i].vars[j].name = variable.Name.Substring(0, variable.Name.IndexOf(":"));
                    savedLayers[i].vars[j].shape = variable.shape.dims;
                    savedLayers[i].vars[j].values = new List<float>();
                    foreach (var np in variable.numpy())
                    {
                        var arr = np.ToArray<float>();
                        savedLayers[i].vars[j].values.AddRange(arr);
                    }
                    j++;
                }
                i++;
            }

            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, savedLayers);
                return ms.ToArray();
            }

        }

        public static Model CreateModel(int num_classes)
        {
            return new AutomataModel (new AutomataModelArgs
            {
                NumClasses = num_classes,
                Activation = keras.activations.Relu
            });

        }

    }
}
