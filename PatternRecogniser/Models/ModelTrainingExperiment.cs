using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace PatternRecogniser.Models
{
    public class ModelTrainingExperiment : Experiment 
    {
        public List<(Bitmap, Pattern, Pattern)> validationSet;
        public double accuracy { get; set; }
        public double precision { get; set; }
        public double recall { get; set; }
        public double specificity { get; set; }
        public double missRate { get; set; }
        public int[,] confusionMatrix { get; set; }
        private int TP;
        private int TN;
        private int FP;
        private int FN;

        public override string GetResults()
        {
            throw new NotImplementedException();
        }

        public override void SaveResult()
        {
            throw new NotImplementedException();
        }
    }
}
