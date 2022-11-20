using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace PatternRecogniser.Models
{
    public abstract class Experiment
    {
        [Key]
        public int experimentId { get; set; }
        public int extendedModelId { get; set; }
        public virtual  ExtendedModel extendedModel { get; set; }// usedModel w diagramie klas

        public virtual ICollection<ExperimentList> experimentLists { get; set; }

        public abstract string GetResults();

        public abstract void SaveResult();
    }

    [Index(nameof(userId), nameof(name), IsUnique = true)]
    public class ExperimentList
    {
        [Key]
        public int experimentListId { get; set; }
        public string name { get; set; }
        public int userId { get; set; }
        public virtual User user { get; set; }
        public virtual ICollection<Experiment> experiments { get; set; }
    }

}
