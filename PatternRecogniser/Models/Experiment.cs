using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PatternRecogniser.Models
{
    public abstract class Experiment
    {
        [Key]
        public int experimentId { get; set; }
        public int extendedModelId { get; set; }

        [ForeignKey("extendedModelId")]
        public virtual  ExtendedModel extendedModel { get; set; }// usedModel w diagramie klas

        public virtual ICollection<ExperimentList> experimentLists { get; set; }

        public abstract string GetResults();

        public abstract void SaveResult();
        public abstract bool IsItMe(string experimentType);
    }

    [Index(nameof(userLogin), nameof(name), IsUnique = true)]
    public class ExperimentList
    {
        [Key]
        public int experimentListId { get; set; }
        public string name { get; set; }
        public string experimentType { get; set; }
        [ForeignKey("User")]
        public string userLogin { get; set; }
        public virtual User user { get; set; }
        public virtual ICollection<Experiment> experiments { get; set; }
    }

}
