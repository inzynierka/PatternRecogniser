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

        public abstract string GetResults();

        public abstract void SaveResult();
    }

    public class ExperimentList
    {
        public string name { get; set; }
        public List<Experiment> experiments;
    }

}
