using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Drawing;
using System.ComponentModel.DataAnnotations;

namespace PatternRecogniser.Models
{
    public abstract class Experiment
    {
        [Key]
        public int experimentId { get; set; }
        public int extendedModelId { get; set; }
        public virtual  ExtendedModel ExtendedModel { get; set; }

        public abstract string GetResults();

        public abstract void SaveResult();
    }

    public class ExperimentList
    {
        [Key]
        public int experimentListId { get; set; }
        public string name { get; set; }
        public int userID { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Experiment> Experiment { get; set; }
    }

}
