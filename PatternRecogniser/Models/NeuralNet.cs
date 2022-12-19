///*****************************************************************************
//   Copyright 2018 The TensorFlow.NET Authors. All Rights Reserved.
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//       http://www.apache.org/licenses/LICENSE-2.0
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//******************************************************************************/

//using System;
//using System.Linq;
////using Tensorflow;
////using Tensorflow.Keras;
////using Tensorflow.Keras.ArgsDefinition;
////using Tensorflow.Keras.Engine;
////using Tensorflow.NumPy;
////using static Tensorflow.Binding;
////using static Tensorflow.KerasApi;


//namespace PatternRecogniser.Models
//{
//    /// <summary>
//    /// Build a convolutional neural network with TensorFlow v2.
//    /// https://github.com/aymericdamien/TensorFlow-Examples/blob/master/tensorflow_v2/notebooks/3_NeuralNetworks/neural_network.ipynb
//    /// </summary>
//    /// // code also used in ExtendedModel.cs
//    /// 

//    // deleted FullyConnectedKeras, but majority of its code was moved to ExtendedModel.cs

//    public class NeuralNet : Model
//    {
//        ILayer fc1;
//        ILayer fc2;
//        ILayer output;

//        public NeuralNet (NeuralNetArgs args) :
//            base (args)
//        {
//            var layers = keras.layers;

//            // First fully-connected hidden layer.
//            fc1 = layers.Dense (args.NeuronOfHidden1, activation: args.Activation1);

//            // Second fully-connected hidden layer.
//            fc2 = layers.Dense (args.NeuronOfHidden2, activation: args.Activation2);

//            output = layers.Dense (args.NumClasses);

//            StackLayers (fc1, fc2, output);
//        }

//        // Set forward pass.
//        protected override Tensors Call (Tensors inputs, Tensor state = null, bool? training = null)
//        {
//            inputs = fc1.Apply (inputs);
//            inputs = fc2.Apply (inputs);
//            inputs = output.Apply (inputs);
//            if (!training.Value)
//                inputs = tf.nn.softmax (inputs);
//            return inputs;
//        }
//    }

//    /// <summary>
//    /// Network parameters.
//    /// </summary>
//    public class NeuralNetArgs : ModelArgs
//    {
//        /// <summary>
//        /// 1st layer number of neurons.
//        /// </summary>
//        public int NeuronOfHidden1 { get; set; }
//        public Activation Activation1 { get; set; }

//        /// <summary>
//        /// 2nd layer number of neurons.
//        /// </summary>
//        public int NeuronOfHidden2 { get; set; }
//        public Activation Activation2 { get; set; }

//        public int NumClasses { get; set; }
//    }
//}
