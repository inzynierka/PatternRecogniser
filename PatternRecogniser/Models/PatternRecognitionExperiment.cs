using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace PatternRecogniser.Models
{
    public class PatternRecognitionExperiment : Experiment
    {
        public Bitmap testedPattern { get; set; }
        public List<(Pattern, double)> recognisedPatterns;

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
