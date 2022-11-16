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
        public virtual  ExtendedModel extendedModel { get; set; }// usedModel w diagramie klas

        public virtual ICollection<ExperimentList> experimentList { get; set; }

        public abstract string GetResults();

        public abstract void SaveResult();
    }

    public class ExperimentList
    {
        [Key]
        public int experimentListId { get; set; }
        public string name { get; set; }
        public int userID { get; set; }
        public virtual User user { get; set; }
        public virtual ICollection<Experiment> experiments { get; set; }
    }

}
