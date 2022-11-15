using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PatternRecogniser.Models
{
    [Index(nameof(email), IsUnique = true)]
    [Index(nameof(login), IsUnique = true)]
    public class User
    {
        [Key]
        public int userId { get; set; }
        [Required]
        public string email { get; set; }
        [Required]
        public string login { get; set; }
        public DateTime createDate { get; set; }
        public DateTime lastLog { get; set; }

        public virtual ICollection<ExtendedModel> extendedModel { get; set; }
        public virtual ICollection<ExperimentList> experimentList { get; set; }

        //public void LoadTrainingSet() { }

        //public void LoadTestingImage() { }

        //public void StartModelTraining() { }

        //public void StartRecognising() { }

        //public void CreateExperimentList() { }

        //public void AddExperiment(Experiment experiment, ExperimentList experimentList)
        //{
        //    foreach (ExperimentList list in experimentLists)
        //    {
        //        if (list == experimentList)
        //        {
        //            list.experiments.Add(experiment);
        //            return;
        //        }
        //    }
        //}

        //public void SaveResult(Experiment experiment) { }

        //public void SaveResultList(ExperimentList experimentList) { }
    }
}
