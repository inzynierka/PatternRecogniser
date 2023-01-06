using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tensorflow;
using Tensorflow.Keras;
using Tensorflow.Keras.ArgsDefinition;
using Tensorflow.Keras.Engine;

namespace PatternRecogniser.Models
{
    public class OurLayer : Layer
    {
        private OurLayerArgs args;

        //private IVariableV1 kernel;
        private int x;

        private IVariableV1 bias;

        private Activation activation => args.Activation;

        public OurLayer (OurLayerArgs args)
            : base (args)
        {
            this.args = args;
            base.SupportsMasking = true;
            int? min_ndim = 2;
            inputSpec = new InputSpec (TF_DataType.DtInvalid, null, min_ndim);
        }

        protected override void build (Tensors inputs)
        {
            long num = inputs.shape.dims.Last ();
            Dictionary<int, int> dictionary = new Dictionary<int, int> ();
            dictionary[-1] = (int)num;
            int? min_ndim = 2;
            Dictionary<int, int> axes = dictionary;
            inputSpec = new InputSpec (TF_DataType.DtInvalid, null, min_ndim, axes);
            x = args.Threshold;
            if (args.UseBias)
            {
                bias = add_weight ("bias", new Shape (args.Units), initializer: args.BiasInitializer, dtype: base.DType);
            }

            built = true;
        }

        protected override Tensors Call (Tensors inputs, Tensor state = null, bool? training = null)
        {
            Tensor tensor = null;
            //int x = 4; // ideolo jakby to było variable, i jakoś się dzięki gradients zmieniało

            foreach (var input in inputs) // jeden input w inputs
            {
                List<float[]> newArrList = new List<float[]> ();
                foreach (var np in input.numpy ())
                {
                    float[] arr = np.ToArray<float> ();
                    int size = (int)Math.Sqrt (arr.GetLength (0));
                    float[,] matrix = new float[size, size];
                    for (int i = 0; i < size; i++)
                    {
                        for (int j = 0; j < size; j++)
                        {
                            matrix[i, j] = arr[i * size + j];
                        }
                    }
                    // zliczamy zapalone komórki
                    int newSize = size - 2;
                    float[] newArr = new float[newSize * newSize];
                    float tmp;
                    for (int i = 0; i < newSize; i++)
                    {
                        for (int j = 0; j < newSize; j++)
                        {
                            int mainIndI = i + 1;
                            int mainIndJ = j + 1;

                            tmp = matrix[mainIndI - 1, mainIndJ - 1] + matrix[mainIndI - 1, mainIndJ] + matrix[mainIndI - 1, mainIndJ + 1] +
                                matrix[mainIndI, mainIndJ - 1] + matrix[mainIndI, mainIndJ] + matrix[mainIndI, mainIndJ + 1] +
                                matrix[mainIndI + 1, mainIndJ - 1] + matrix[mainIndI + 1, mainIndJ] + matrix[mainIndI + 1, mainIndJ + 1];
                            
                            if (tmp > x)
                            {
                                newArr[i * newSize + j] = 1;
                            }
                            else
                            {
                                newArr[i * newSize + j] = 0;
                            }
                        }
                    }
                    newArrList.Add (newArr);
                }
                tensor = ops.convert_to_tensor (newArrList.ToArray ());
            }

            if (args.UseBias)
            {
                tensor = Binding.tf.nn.bias_add (tensor, bias);
            }

            if (args.Activation != null)
            {
                tensor = activation (tensor);
            }

            return tensor;
        }

        public static OurLayer from_config (LayerArgs args)
        {
            return new OurLayer (args as OurLayerArgs);
        }
    }

    // args analogiczne z tymi dla Dense
    public class OurLayerArgs : LayerArgs
    {
        //
        // Summary:
        //     Positive integer, rozmiar inputów po zmianach, tzn. najpierw 26*26, potem 24*24 itd. aż dojdziemy do 16*16
        public int Units { get; set; }

        public int Threshold { get; set; }

        //
        // Summary:
        //     Activation function to use.
        public Activation Activation { get; set; }

        //
        // Summary:
        //     Whether the layer uses a bias vector.
        public bool UseBias { get; set; } = true;


        //
        // Summary:
        //     Initializer for the `kernel` weights matrix.
        public IInitializer KernelInitializer { get; set; } = Binding.tf.glorot_uniform_initializer;


        //
        // Summary:
        //     Initializer for the bias vector.
        public IInitializer BiasInitializer { get; set; } = Binding.tf.zeros_initializer;


        //
        // Summary:
        //     Regularizer function applied to the `kernel` weights matrix.
        public IRegularizer KernelRegularizer { get; set; }

        //
        // Summary:
        //     Regularizer function applied to the bias vector.
        public IRegularizer BiasRegularizer { get; set; }

        //
        // Summary:
        //     Constraint function applied to the `kernel` weights matrix.
        public Action KernelConstraint { get; set; }

        //
        // Summary:
        //     Constraint function applied to the bias vector.
        public Action BiasConstraint { get; set; }
    }
}
