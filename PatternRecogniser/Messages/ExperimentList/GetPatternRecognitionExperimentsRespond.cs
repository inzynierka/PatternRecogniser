using PatternRecogniser.Models;
using System.Collections;
using System.Collections.Generic;

namespace PatternRecogniser.Messages.ExperimentList
{
    public class GetPatternRecognitionExperimentsRespond
    {
        public List<Experiment> experiments;
        public object extendedModel;
        public GetPatternRecognitionExperimentsRespond(List<Experiment> experiments, object extendedModel)
        {
            this.experiments = experiments;
            foreach (var experiment in experiments)
            {
                var pre = (PatternRecognitionExperiment)experiment;
                pre.extendedModel = null;
                foreach (var recognisedPattern in pre.recognisedPatterns)
                    recognisedPattern.pattern = null;
            }
            this.extendedModel = extendedModel;
        }
    }
}
