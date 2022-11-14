using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace PatternRecogniser.Models
{
    public enum DistributionType
    {
        TrainTest, CrossValidation
    }

    public class ExtendedModel
    {
        public string name { get; set; }
        public List<Pattern> patterns { get; set; }
        public ModelTrainingExperiment statistics { get; set; }
        public DistributionType distribution { get; set; }
        //private Model model; 

        public void TrainModel(DistributionType distribution)
        {
            this.distribution = distribution;
        }

        public void TrainModelTrainTest(PatternData data, int train, int test) { }

        public void TrainModelCrossValidation(PatternData data, int n) { }

        public string RecognisePattern(Bitmap picture)
        {
            return ""; // returns a string that follows json formatting
        }

        private void TrainIndividualModel(PatternData train, PatternData test) { }
    }
}
