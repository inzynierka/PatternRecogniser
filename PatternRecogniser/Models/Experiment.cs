using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Drawing;

namespace PatternRecogniser.Models
{
    public abstract class Experiment
    {
        public ExtendedModel usedModel { get; set; }

        public string GetResults()
        {
            return ""; // returns a string that follows json formatting
        }

        public void SaveResult() { }
    }

    public class ExperimentList
    {
        public string name { get; set; }
        public List<Experiment> experiments;
    }

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
    }

    public class PatternRecognitionExperiment : Experiment
    {
        public Bitmap testedPattern { get; set; }
        public List<(Pattern, double)> recognisedPatterns;
    }
}
