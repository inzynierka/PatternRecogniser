﻿using PatternRecogniser.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PatternRecogniser.Messages.TrainModel
{
    public class ModelDetalisRespond
    {
        public double accuracy { get; set; }
        public double precision { get; set; }
        public double recall { get; set; }
        public double specificity { get; set; }
        public double missRate { get; set; }
        public double F1 { get; set; }
        public int[,] confusionMatrix { get; set; } 
        public string serializedRoc { get; set; }
        public List<ValidationSet> validationSet { get; set; }
        public List<Pattern> patterns { get; set; }
        public string distributionType { get; set; }

        public ModelDetalisRespond(ModelTrainingExperiment mte, ICollection<Pattern> ps, DistributionType distributionType)
        {
            accuracy = mte.accuracy;
            precision = mte.precision;
            recall = mte.recall;
            specificity = mte.specificity;
            missRate = mte.missRate;
            F1 = mte.F1;
            confusionMatrix = saveConfusionMatrixAs2DimArray(mte.confusionMatrix);
            validationSet = mte.validationSet.ToList();
            patterns = ps.ToList();
            this.distributionType = distributionType.ToString();
            serializedRoc = mte.serializedRoc;

            foreach (var validation in validationSet)
            {
                validation.modelTrainingExperiment = null;
                validation.truePattern = null;
                validation.recognisedPattern = null;
            }
            foreach (var pattern in patterns)
            {
                pattern.extendedModel = null;
            }
        }

        private int[,] saveConfusionMatrixAs2DimArray(int[] confusionMatrix)
        {
            int lenght = confusionMatrix.Length;
            int dim = (int) Math.Sqrt(lenght);
            int[,] ConfusionMatrix2Dim = new int[dim, dim];
            for(int i = 0; i < dim; i++)
            {
                for(int j = 0; j < dim; j++)
                {
                    ConfusionMatrix2Dim[i, j] = confusionMatrix[i*dim + j];
                }
            }
            return ConfusionMatrix2Dim;
        }
    }
}
