using Tensorflow.NumPy;

namespace PatternRecogniser.Messages.Model
{
    public class ExtendedModelStringMessages
    {
        public string incorectFileStructure => "Zła struktura pliku";
        public string startTraining => "Rozpoczęto trenowanie";
        public string tooSmalSetsNumber => "Ilość zbiorów musi być conajmniej równa 2";
        public string tooLargeSetsNumber => "Za duży podział zborów";
        public string crossValidationModelTraining(int j) => $"Walidacja krzyżowa - trenowanie modelu {j}";
        public string trainingProggres(int step, int training_steps, NDArray lossNumpy, NDArray accNumpy) => $"Trenowanie - krok {step} z {training_steps}\nStrata: {lossNumpy}\nDokładność: {accNumpy}";
        public string startValidation => "Rozpoczęto walidację";
    }
}
