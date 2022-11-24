using PatternRecogniser.Models;

namespace PatternRecogniser.Messages.TrainModel.Responds
{
    public class ModelStatistics
    {
        public ModelStatistics(ModelTrainingExperiment modelTrainingExperiment)
        {
            accuracy = modelTrainingExperiment.accuracy;
            precision = modelTrainingExperiment.precision;
            recall = modelTrainingExperiment.recall;
            specificity = modelTrainingExperiment.specificity;
            missRate = modelTrainingExperiment.missRate;
            confusionMatrix = modelTrainingExperiment.confusionMatrix;
            confusionMatrix = modelTrainingExperiment.confusionMatrix;

        }

        public double accuracy { get; set; }
        public double precision { get; set; }
        public double recall { get; set; }
        public double specificity { get; set; }
        public double missRate { get; set; }
        public int[] confusionMatrix { get; set; } // zmieniłem by umożliwić mapowanie

    }
}
