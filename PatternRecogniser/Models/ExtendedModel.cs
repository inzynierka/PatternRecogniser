using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PatternRecogniser.ThreadsComunication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Threading;
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
        public virtual ICollection<Experiment> experiments { get; set; }

        //private Model model; 

        // tymczasowo asynchroniczna w celu testowania
        public async Task TrainModel(DistributionType distribution, ITrainingUpdate trainingUpdate, IFormFile trainingSet, CancellationToken stoppingToken) // nie potrzebne CancellationToken w późniejszym programie
        {

            this.distribution = distribution;
            // trenowanie 
            for (int i = 0; i < 3; i++)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                trainingUpdate.Update($"info dla usera {userLogin}: {DateTime.Now}\n"); // zapisuje info
            }
            // trenowanie

            //validaja
            await Task.Delay(TimeSpan.FromSeconds(3));
            trainingUpdate.Update($"info dla usera {userLogin}: start validacji {DateTime.Now}\n"); // zapisuje info
            var experyment = new ModelTrainingExperiment()
            {
                extendedModel = this
            };
            modelTrainingExperiment = experyment;
            //validacja
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
