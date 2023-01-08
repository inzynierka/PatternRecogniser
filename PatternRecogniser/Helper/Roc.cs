using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using Tensorflow;
using Newtonsoft.Json;

namespace PatternRecogniser.Helper
{
    [Serializable]
    public class RocOvR
    {
        public Roc[] rocs { get; set; }

        [JsonConstructor]
        public RocOvR(Roc[] rocs) 
        {
            this.rocs = rocs;
        }
        public RocOvR(Tensor predictions, Tensor trueLabels, int labelCount, string[] labels)
        {
            rocs = new Roc[labels.Length];
            for (int i = 0; i < labelCount; i++)
            {
                // tutaj wstawić odpowiednią etykiete
                rocs[i] = createRocOneVsRest(predictions, trueLabels, i, 15, labels[i]);
            }
        }
        private Roc createRocOneVsRest(Tensor predictions, Tensor trueLabels, int selectedTrueLable, int pointsNumber, string label)
        {
            int[] trueLabelsArr = new int[trueLabels.size];
            float[] selectedTrueLaabelProbality = new float[trueLabels.size];
            int i = 0;
            // Tensor to array
            foreach (var l in trueLabels.numpy())
            {
                if (selectedTrueLable == l)
                    trueLabelsArr[i] = 1;
                else
                    trueLabelsArr[i] = 0;
                i++;
            }
            i = 0;

            // prawdopodobieństwo wybranej labelki
            foreach (var np in predictions.numpy())
            {
                selectedTrueLaabelProbality[i] = np[selectedTrueLable];
                i++;
            }

            return new Roc(selectedTrueLaabelProbality, trueLabelsArr, pointsNumber, label);
        }
    }
    [Serializable]
    public class Roc
    {
        public string label { get; set; }
        public float[] tpr { get; set; }
        public float[] fpr { get; set; }

        [JsonConstructor]
        public Roc(float[] recall, float[] fpr, string label)
        {
            this.tpr = recall;
            this.fpr = fpr;
            this.label = label;
        }
        public Roc(float[] trueLabelProbality, int[] trueLabels, int pointsNumber, string label)
        {
            this.label = label;
            int tp, fp, tn, fn;
            tpr = new float[pointsNumber];
            fpr = new float[pointsNumber];
            for (int i = 0; i < pointsNumber; i++)
            {
                tp = fp = tn = fn = 0;
                float axis = i /(float) pointsNumber;
                for (int j = 0; j < trueLabelProbality.Count(); j++)
                {
                    if (trueLabels[j] == 1)
                    {
                        if (trueLabelProbality[i] > axis)
                            tp++;
                        else
                            fn++;
                    }
                    else
                    {
                        if (trueLabelProbality[i] > axis)
                            fp++;
                        else
                            tn++;
                    }
                }
                tpr[i] = tp == 0 ? 0 : tp / (float)(tp + fn);
                fpr[i] = fp == 0 ? 0 : fp / (float)(fp + tn);
            }
        }
    }
}
