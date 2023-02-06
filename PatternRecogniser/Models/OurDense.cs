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
    //
    // Summary:
    //     Just your regular densely-connected NN layer.
    // Layer from Tensorflow.NET but with public kernel and bias
    public class OurDense : Layer
    {
        private DenseArgs args;

        public IVariableV1 kernel;

        public IVariableV1 bias;

        private Activation activation => args.Activation;

        public OurDense (DenseArgs args)
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
            kernel = add_weight ("kernel", new Shape (num, args.Units), initializer: args.KernelInitializer, dtype: base.DType);
            if (args.UseBias)
            {
                bias = add_weight ("bias", new Shape (args.Units), initializer: args.BiasInitializer, dtype: base.DType);
            }

            built = true;
        }

        protected override Tensors Call (Tensors inputs, Tensor state = null, bool? training = null)
        {
            Tensor tensor = null;
            int rank = inputs.rank;
            tensor = ((rank <= 2) ? gen_math_ops.mat_mul (inputs, kernel.AsTensor ()) : Binding.tf.linalg.tensordot (inputs, kernel.AsTensor (), new int[2, 1]
            {
                { rank - 1 },
                { 0 }
            }));
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

        public static OurDense from_config (LayerArgs args)
        {
            return new OurDense (args as DenseArgs);
        }
    }
}
