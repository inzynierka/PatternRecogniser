using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace PatternRecogniser.Models
{
    public class ModelTrainingExperiment : Experiment 
    {
        public int validationSetId;
        public double accuracy { get; set; }
        public double precision { get; set; }
        public double recall { get; set; }
        public double specificity { get; set; }
        public double missRate { get; set; }
        public int[,] confusionMatrix { get; set; }
        private int TP { get; set; }
        private int TN { get; set; }
        private int FP { get; set; }
        private int FN { get; set; }

        public virtual ValidationSet validationSet { get; set; }

        public override string GetResults()
        {
            throw new NotImplementedException();
        }

        public override void SaveResult()
        {
            throw new NotImplementedException();
        }
    }

    public class ValidationSet
    {
        public int validationSetId { get; set; }
        public Bitmap testedPattern { get; set; }
        public int truePatternId { get; set; }
        public int regognizedPatternId { get; set; }

        public virtual Pattern truePattern { get; set; }
        public virtual Pattern regognizedPattern { get; set; }

    }
}
