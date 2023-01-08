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

using System;
using System.Collections.Generic;
using System.Linq;
using Tensorflow;
using Tensorflow.Keras;
using Tensorflow.Keras.ArgsDefinition;
using Tensorflow.Keras.Engine;
using Tensorflow.NumPy;
using static Tensorflow.Binding;
using static Tensorflow.KerasApi;


namespace PatternRecogniser.Models
{
    /// <summary>
    /// Build a convolutional neural network with TensorFlow v2.
    /// https://github.com/aymericdamien/TensorFlow-Examples/blob/master/tensorflow_v2/notebooks/3_NeuralNetworks/neural_network.ipynb
    /// </summary>
    /// // code also used in ExtendedModel.cs
    /// 

    // deleted FullyConnectedKeras, but majority of its code was moved to ExtendedModel.cs

    internal class AutomataModel : Model
    {
        ILayer fc1;
        ILayer fc2;
        ILayer fc3;
        ILayer fc4;
        ILayer fc5;
        ILayer fc6;

        ILayer dense;
        ILayer output;

        public AutomataModel (AutomataModelArgs args) :
            base (args)
        {
            var layers = keras.layers;
            float[,] rule1 = new float[5, 5]
            {
                { 0, 0, 1, 0, 0 },
                { 0, 0, 1, 0, 0 },
                { 0, 0, 1, 0, 0 },
                { 0, 0, 1, 0, 0 },
                { 0, 0, 1, 0, 0 }
            };
            float[,] rule2 = new float[5, 5]
            {
                { 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0 },
                { 1, 1, 1, 1, 1 },
                { 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0 }
            };
            float[,] rule3 = new float[5, 5]
            {
                { 1, 0, 0, 0, 0 },
                { 0, 1, 0, 0, 0 },
                { 0, 0, 1, 0, 0 },
                { 0, 0, 0, 1, 0 },
                { 0, 0, 0, 0, 1 }
            };
            float[,] rule4 = new float[5, 5]
            {
                { 0, 0, 0, 0, 1 },
                { 0, 0, 0, 1, 0 },
                { 0, 0, 1, 0, 0 },
                { 0, 1, 0, 0, 0 },
                { 1, 0, 0, 0, 0 }
            };
            float[,] rule5 = new float[5, 5]
            {
                { 0, 0, 1, 0, 0 },
                { 0, 0, 1, 0, 0 },
                { 1, 1, 1, 1, 1 },
                { 0, 0, 1, 0, 0 },
                { 0, 0, 1, 0, 0 }
            };
            float[,] rule6 = new float[5, 5]
            {
                { 1, 0, 0, 0, 1 },
                { 0, 1, 0, 1, 0 },
                { 0, 0, 1, 0, 0 },
                { 0, 1, 0, 1, 0 },
                { 1, 0, 0, 0, 1 }
            };

            fc1 = AutomataConvLayer (24 * 24, rule1, 5, activation: args.Activation);
            fc2 = AutomataConvLayer (24 * 24, rule2, 5, activation: args.Activation);
            fc3 = AutomataConvLayer (24 * 24, rule3, 5, activation: args.Activation);
            fc4 = AutomataConvLayer (24 * 24, rule4, 5, activation: args.Activation);
            fc5 = AutomataConvLayer (24 * 24, rule5, 8, activation: args.Activation);
            fc6 = AutomataConvLayer (24 * 24, rule6, 8, activation: args.Activation);

            //dense = layers.Dense (20);
            output = layers.Dense (args.NumClasses, activation: "softmax");

            StackLayers (fc1, fc2, fc3, fc4, fc5, fc6, output);// fc7, fc8, fc9, fc10, fc11, output);
        }

        protected override Tensors Call (Tensors inputs, Tensor state = null, bool? training = null)
        {
            //inputs = fc1.Apply (inputs);
            Tensors result1 = fc1.Apply (inputs);
            Tensors result2 = fc2.Apply (inputs);
            Tensors result3 = fc3.Apply (inputs);
            Tensors result4 = fc4.Apply (inputs);
            Tensors result5 = fc5.Apply (inputs);
            Tensors result6 = fc6.Apply (inputs);

            inputs = JoinResults (new List<Tensors> { result1, result2, result3, result4, result5, result6 });

            //inputs = dense.Apply (inputs);
            inputs = output.Apply (inputs);

            if (!training.Value)
                inputs = tf.nn.softmax (inputs);
            return inputs;
        }

        private Tensors JoinResults (List<Tensors> results)
        {
            int howManyToJoin = results.Count ();
            Tensors outputs = new Tensors ();
            int count = results[0].Count ();

            for (int i = 0; i < count; i++)
            {
                List<List<float[]>> allResults = new List<List<float[]>> ();
                for (int j = 0; j < howManyToJoin; j++)
                {
                    List<float[]> allResultsMini = new List<float[]> ();
                    foreach (var np in results[j][i].numpy ())
                    {
                        float[] arr = np.ToArray<float> ();
                        allResultsMini.Add (arr);
                    }
                    allResults.Add (allResultsMini);
                }

                List<float[]> newResults = new List<float[]> ();
                int resultsPerOne = allResults[0].Count ();
                for (int j = 0; j < resultsPerOne; j++)
                {
                    List<float> newArr = new List<float> ();
                    foreach (var res in allResults)
                    {
                        newArr.AddRange (res[j]);
                    }
                    newResults.Add (newArr.ToArray ());
                }

                outputs.Add (ops.convert_to_tensor (newResults.ToArray ()));
            }


            return outputs;
        }

        private Tensors JoinResults (Tensors result1, Tensors result2)
        {
            int count = result1.Count ();
            // result2.Count() powinno być równe

            Tensors outputs = new Tensors ();
            for (int i = 0; i < count; i++)
            {
                List<float[]> allResults1 = new List<float[]> ();
                foreach (var np in result1[i].numpy ())
                {
                    float[] arr = np.ToArray<float> ();
                    allResults1.Add (arr);
                }

                List<float[]> allResults2 = new List<float[]> ();
                foreach (var np in result2[i].numpy ())
                {
                    float[] arr = np.ToArray<float> ();
                    allResults2.Add (arr);
                }

                // łączymy
                List<float[]> allResults = new List<float[]> ();
                for (int j = 0; j < allResults1.Count; j++)
                {
                    float[] newArr = new float[allResults1[j].Length];

                    for (int k = 0; k < allResults1[j].Length; k++)
                    {
                        if (allResults1[j][k] == 1 || allResults2[j][k] == 1)
                            newArr[k] = 1;
                        else
                            newArr[k] = 0;
                    }

                    allResults.Add (newArr);
                }

                outputs.Add (ops.convert_to_tensor (allResults.ToArray ()));
            }

            return outputs;
        }

        public AutomataConvLayer AutomataConvLayer (int units, float[,] rule, int threshold, Activation activation = null, Shape input_shape = null)
        {
            return new AutomataConvLayer (new AutomataConvLayerArgs
            {
                Units = units,
                Rule = rule,
                Threshold = threshold,
                Activation = (activation ?? KerasApi.keras.activations.Linear),
                InputShape = input_shape
            });
        }
    }

    public class AutomataModelArgs : ModelArgs
    {
        public Activation Activation { get; set; }

        public int NumClasses { get; set; }
    }
}
