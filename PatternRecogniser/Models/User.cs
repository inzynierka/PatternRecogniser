using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatternRecogniser.Models
{
    public class User
    {
        public string email { get; set; }
        public string login { get; set; }
        public List<ExtendedModel> modelList;
        public List<ExperimentList> experimentLists;

        public void LoadTrainingSet() { }

        public void LoadTestingImage() { }

        public void StartModelTraining() { }

        public void StartRecognising() { }

        public void CreateExperimentList() { }

        public void AddExperiment(Experiment experiment, ExperimentList experimentList)
        {
            foreach(ExperimentList list in experimentLists)
            {
                if (list == experimentList)
                {
                    list.experiments.Add(experiment);
                    return;
                }
            }
        }

        public void SaveResult(Experiment experiment) { }

        public void SaveResultList(ExperimentList experimentList) { }
    }
}
