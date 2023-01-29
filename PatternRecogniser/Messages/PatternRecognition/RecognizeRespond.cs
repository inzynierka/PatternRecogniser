using PatternRecogniser.Models;
using System.Collections.Generic;
using System.Linq;

namespace PatternRecogniser.Messages.PatternRecognition
{
    public class RecognizeRespond
    {
        public byte[] testedPattern { get; set; }
        public  List<RecognisedPatterns> recognisedPatterns { get; set; } 

        public RecognizeRespond(PatternRecognitionExperiment pre)
        {
            testedPattern = pre.testedPattern;
            recognisedPatterns = pre.recognisedPatterns.ToList();
            foreach(var recognisedPattern in recognisedPatterns)
            {
                recognisedPattern.pattern.extendedModel = null;
            }
        }
    }
}
