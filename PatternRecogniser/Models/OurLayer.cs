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
    public class AutomataConvLayer : Layer
    {
        private AutomataConvLayerArgs args;

        private float[,] rule;

        private int threshold;

        private Activation activation => args.Activation;

        public AutomataConvLayer (AutomataConvLayerArgs args)
            : base (args)
        {
            this.args = args;
            base.SupportsMasking = false;
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
            rule = args.Rule;
            threshold = args.Threshold;

            built = true;
        }

        private float calculateRuleOnMatrix (float[,] matrix, int mainIndI, int mainIndJ)
        {
            return matrix[mainIndI - 2, mainIndJ - 2] * rule[0, 0] + matrix[mainIndI - 2, mainIndJ - 1] * rule[0, 1] + matrix[mainIndI - 2, mainIndJ] * rule[0, 2] + matrix[mainIndI - 2, mainIndJ + 1] * rule[0, 3] + matrix[mainIndI - 2, mainIndJ + 2] * rule[0, 4] +
                   matrix[mainIndI - 1, mainIndJ - 2] * rule[1, 0] + matrix[mainIndI - 1, mainIndJ - 1] * rule[1, 1] + matrix[mainIndI - 1, mainIndJ] * rule[1, 2] + matrix[mainIndI - 1, mainIndJ + 1] * rule[1, 3] + matrix[mainIndI - 1, mainIndJ + 2] * rule[1, 4] +
                   matrix[mainIndI    , mainIndJ - 2] * rule[2, 0] + matrix[mainIndI    , mainIndJ - 1] * rule[2, 1] + matrix[mainIndI    , mainIndJ] * rule[2, 2] + matrix[mainIndI    , mainIndJ + 1] * rule[2, 3] + matrix[mainIndI    , mainIndJ + 2] * rule[2, 4] +
                   matrix[mainIndI + 1, mainIndJ - 2] * rule[3, 0] + matrix[mainIndI + 1, mainIndJ - 1] * rule[3, 1] + matrix[mainIndI + 1, mainIndJ] * rule[3, 2] + matrix[mainIndI + 1, mainIndJ + 1] * rule[3, 3] + matrix[mainIndI + 1, mainIndJ + 2] * rule[3, 4] +
                   matrix[mainIndI + 2, mainIndJ - 2] * rule[4, 0] + matrix[mainIndI + 2, mainIndJ - 1] * rule[4, 1] + matrix[mainIndI + 2, mainIndJ] * rule[4, 2] + matrix[mainIndI + 2, mainIndJ + 1] * rule[4, 3] + matrix[mainIndI + 2, mainIndJ + 2] * rule[4, 4];
        }

        protected override Tensors Call (Tensors inputs, Tensor state = null, bool? training = null)
        {
            Tensor tensor = null;
            foreach (var input in inputs)
            {
                List<float[]> newArrList = new List<float[]> ();
                foreach (var np in input.numpy ())
                {
                    // zmiana wejścia na macierz
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

                    // konwolucja
                    int newSize = size - rule.GetLength (0) + 1; // chyba
                    float[] newArr = new float[newSize * newSize];
                    float tmp;
                    for (int i = 0; i < newSize; i++)
                    {
                        for (int j = 0; j < newSize; j++)
                        {
                            int mainIndI = i + 2;
                            int mainIndJ = j + 2;

                            tmp = calculateRuleOnMatrix (matrix, mainIndI, mainIndJ);

                            if (tmp >= threshold)
                                newArr[i * newSize + j] = 1;
                            else
                                newArr[i * newSize + j] = 0;
                        }
                    }

                    newArrList.Add (newArr);
                }
                tensor = ops.convert_to_tensor (newArrList.ToArray ());
            }

            if (args.Activation != null)
            {
                tensor = activation (tensor);
            }

            return tensor;
        }

        public static AutomataConvLayer from_config (LayerArgs args)
        {
            return new AutomataConvLayer (args as AutomataConvLayerArgs);
        }
    }

    public class AutomataConvLayerArgs : LayerArgs
    {
        //
        // Summary:
        //     Positive integer, rozmiar inputów po zmianach, tzn. najpierw 26*26, potem 24*24 itd. aż dojdziemy do 16*16
        public int Units { get; set; }

        public float[,] Rule { get; set; }

        public int Threshold { get; set; }

        //
        // Summary:
        //     Activation function to use.
        public Activation Activation { get; set; }
    }
}
