using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace PatternRecogniser.Models
{
    [Table("PatternRecognitionExperiment")]
    public class PatternRecognitionExperiment : Experiment
    {
        public byte[] testedPattern { get; set; }
        public virtual ICollection<RecognisedPatterns> recognisedPatterns { get; set; } // albo json

        public override string GetResults()
        {
            throw new NotImplementedException();
        }

        public override bool IsItMe(string experimentType)
        {
            return "PatternRecognitionExperiment"==experimentType;
        }

        public override void SaveResult()
        {
            throw new NotImplementedException();
        }
    }


    public  class RecognisedPatterns
    {
        [Key]
        public int recognisedPatternsId { get; set; }
        public int patternId { get; set; }
        public double probability { get; set; }
        public int PatternRecognitionExperimentexperimentId { get; set; }
        public virtual Pattern pattern { get; set; }
    }

}
