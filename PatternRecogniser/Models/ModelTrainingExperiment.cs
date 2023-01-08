using PatternRecogniser.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Tensorflow;
using Tensorflow.NumPy;

namespace PatternRecogniser.Models
{
    [Table("ModelTrainingExperiment")]
    public class ModelTrainingExperiment : Experiment 
    {
        public double accuracy { get; set; }
        public double precision { get; set; }
        public double recall { get; set; }
        public double specificity { get; set; }
        public double missRate { get; set; }
        public int[] confusionMatrix { get; set; } // zmieniłem by umożliwić mapowanie
        public string serializedRocs { get; set; }
        private int TP { get; set; }
        private int TN { get; set; }
        private int FP { get; set; }
        private int FN { get; set; }

        public virtual ICollection<ValidationSet> validationSet { get; set; }

        public ModelTrainingExperiment (Tensor predictions, Tensor trueLabels, int labelCount)
        {
            var tmpLabels = new string[labelCount];
            for (int i = 0; i < labelCount; i++) { tmpLabels[i] = i.ToString(); }
            RocOvR rocOvR = new RocOvR(predictions, trueLabels, labelCount, tmpLabels);
            serializedRoc = JsonSerializer.Serialize<RocOvR>(rocOvR);
            int[,] confMatrix2D = createConfusionMatrix (predictions, trueLabels, labelCount);
            saveConfusionMatrixAs1DimArray (confMatrix2D);
            calculateTP (confMatrix2D);
            calculateTN (confMatrix2D);
            calculateFP (confMatrix2D);
            calculateFN (confMatrix2D);
            calculateAccuracy ();
            calculatePrecision ();
            calculateRecall ();
            calculateSpecificity ();
        }
        public ModelTrainingExperiment () { }

        private int[,] createConfusionMatrix (Tensor predictions, Tensor trueLabels, int labelCount)
        {
            int[,] result = new int[labelCount, labelCount];
            int[] trueLabelsArr = new int[trueLabels.size];
            int[] mainPrediction = new int[trueLabels.size];
            int i = 0;
            // Tensor to array
            foreach (var label in trueLabels.numpy())
            {
                trueLabelsArr[i] = label;
                i++;
            }
            i = 0;

            // find main prediction 
            foreach (var np in predictions.numpy ())
            {
                int[] preds = new int[labelCount];
                int j = 0;
                foreach (var pred in np)
                {
                    preds[j] = pred;
                    j++;
                }
                int indMax = 0, max = 0;
                for (int k = 0; k < labelCount; k++)
                {
                    if (preds[k] > max)
                    {
                        indMax = k;
                        max = preds[k];
                    }
                }
                mainPrediction[i] = indMax;
                i++;
            }


            for (i = 0; i < trueLabelsArr.Length; i++)
            {
                result[mainPrediction[i], trueLabelsArr[i]]++;
            }

            return result;
        }
        private void saveConfusionMatrixAs1DimArray(int[,] confMatrix2D)
        {
            int length = confMatrix2D.GetLength (0);
            confusionMatrix = new int[length * length];
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    confusionMatrix[i * length + j] = confMatrix2D[i, j];
                }
            }
        }
        private void calculateTP (int[,] matrix)
        {
            TP = 0;
            int count = matrix.GetLength (0);

            for (int i = 0; i < count; i++)
            {
                TP += matrix[i, i];
            }
        }
        private void calculateTN (int[,] matrix)
        {
            TN = 0;
            int count = matrix.GetLength (0);
            int all = 0;

            for (int j = 0; j < count; j++)
            {
                for (int k = 0; k < count; k++)
                {
                    all += matrix[j, k];
                }
            }
            all *= count - 2;
            TN = all + TP; // TP - zsumowane wartości na diagonali
        }
        private void calculateFP (int[,] matrix)
        {
            FP = 0;
            int count = matrix.GetLength (0);

            for (int i = 0; i < count; i++)
            {
                // i - prediction
                int miniFP = 0;
                for (int j = 0; j < count; j++)
                {
                    if (j != i)
                    {
                        miniFP += matrix[i, j];
                    }
                }
                FP += miniFP;
            }
        }
        private void calculateFN (int[,] matrix)
        {
            FN = 0;
            int count = matrix.GetLength (0);

            for (int i = 0; i < count; i++)
            {
                // i - actual
                int miniFN = 0;
                for (int j = 0; j < count; j++)
                {
                    if (j != i)
                    {
                        miniFN += matrix[j, i];
                    }
                }
                FN += miniFN;
            }
        }
        private void calculateAccuracy ()
        {
            if (TP == 0 && TN == 0 && FP == 0 && FN == 0)
            {
                accuracy = 0;
            }
            else
            {
                accuracy = (float)(TP + TN) / (float)(TP + TN + FP + FN);
            }
        }
        private void calculatePrecision ()
        {
            if (TP == 0 && FP == 0)
            {
                precision = 0;
            }
            else
            {
                precision = (float)(TP) / (float)(TP + FP);
            }
        }
        private void calculateRecall ()
        { 
            if (TP == 0 && FN == 0)
            {
                recall = 0;
            }
            else
            {
                recall = (float)(TP) / (float)(TP + FN);
            }
        }
        private void calculateSpecificity ()
        {
            if (TN == 0 && FP == 0)
            {
                specificity = 0;
            }
            else
            {
                specificity = (float)(TN) / (float)(TN + FP);
            }
        }

        

        public override string GetResults()
        {
            throw new NotImplementedException();
        }

        public override void SaveResult()
        {
            throw new NotImplementedException();
        }

        public override bool IsItMe(string experimentType)
        {
            return experimentType == "ModelTrainingExperiment";
        }
    }

    public class ValidationSet
    {
        public int validationSetId { get; set; }
        public byte[] testedPattern { get; set; }
        public int truePatternId { get; set; }
        public int recognisedPatternId { get; set; }
        public int experimentId { get; set; }

        [ForeignKey("experimentId")]
        public virtual ModelTrainingExperiment modelTrainingExperiment  { get; set;}
        public virtual Pattern truePattern { get; set; }
        public virtual Pattern recognisedPattern { get; set; }

    }
}
