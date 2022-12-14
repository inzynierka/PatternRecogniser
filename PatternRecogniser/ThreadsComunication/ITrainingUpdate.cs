using System.Text;

namespace PatternRecogniser.ThreadsComunication
{
    public interface ITrainingUpdate
    {
        // Dopisuje nowe info
        public void Update(string update);
        // Pobiera najnowsze info
        public string ActualInfo(string login, string modelName);
        // Ustalamy dla kogo jest komunikat
        public void SetNewUserModel(string login, string modelName);

        public bool IsUserModelInTraining(string login, string modelName);
        public bool IsUserTrainingModel(string login);

    }

    public class SimpleComunicationOneToMany: ITrainingUpdate
    {
        private string  _login { get; set; }
        private string _modelName;
        private StringBuilder _infoMaker = new StringBuilder();
        private string _infoPublisher;

        public string ActualInfo(string login ,string modelName)
        {
            if(login == this._login)
                return _infoPublisher;
            else
                return string.Empty;
        }

        public void SetNewUserModel(string login, string modelName)
        {
            // lokuje by nikt nie dostał nie swoje info, potencjalnie nie potrzebne
            lock (this) 
            { 
                _login = login;
                _modelName = modelName;
                _infoMaker.Clear();
                _infoPublisher = string.Empty;
            }
        }

        public void Update(string update)
        {
            // bezpiecznie bo tylko jeden wątek zapisuje
            _infoMaker.Append(update);
            // bezpiecznie bo zmieniamy tylko referencje 
            _infoPublisher = _infoMaker.ToString();
        }

        public bool IsUserModelInTraining(string login, string modelName)
        {
            return _login == login && _modelName == modelName;
        }

        public bool IsUserTrainingModel(string login)
        {
            return _login == login;
        }
    }
}
