using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace PatternRecogniser.Models
{
    public enum DistributionType
    {
        TrainTest, CrossValidation
    }

    

    [Index(nameof(userLogin), nameof(name), IsUnique = true)] 
    public class ExtendedModel
    {
        [Key]
        public int extendedModelId { get; set; }
        [Required]
        [ForeignKey("User")]
        public string userLogin { get; set; }
        public string name { get; set; }
        public DistributionType distribution { get; set; }

        public virtual User user { get; set; }
        public virtual ICollection<Pattern> patterns { get; set; }
        public virtual ModelTrainingExperiment modelTrainingExperiment { get; set; } // statistics w diagramie klas

        //private Model model; 

        public void TrainModel(DistributionType distribution)
        {
            this.distribution = distribution;
        }

        public void TrainModelTrainTest(PatternData data, int train, int test) { }

        public void TrainModelCrossValidation(PatternData data, int n) { }

        public List<RecognisedPatterns> RecognisePattern(Bitmap picture)
        {
            return new List<RecognisedPatterns>(); // returns a string that follows json formatting
        }

        private void TrainIndividualModel(PatternData train, PatternData test) { }
    }
}
