namespace PatternRecogniser.Messages.TrainModel
{
    public class TrainModelStringMessages
    {
        public string modelAlreadyExist => "Model już istnieje";
        public string incorectFileFormat => "Zły format pliku";
        public string youAreNotInQueue => "Nie ma cię w kolejce";
        public string deletedFromQueue => "Usunięto";
        public string failedToDelete => "Nie udało się usunąć";
        public string didNotEnterLoginOrModelName => "Nie podano nazwy modelu lub loginu";
        public string modelIsTrained => "Model nie jest trenowany";
        public string modelIsInQueue => "Model jest w kolejce";
        public string modelTrainingComplete => "Model jest wytrenowany (znajduje się w zakładce \"Moje Modele\")";
        public string modelTrainingFailed => "Nie udało się wytrenować modelu";
        public string modelNotFound => "Nie znaleziono podanego modelu";
    }
}
