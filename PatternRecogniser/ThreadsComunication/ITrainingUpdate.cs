using System.Text;

namespace PatternRecogniser.ThreadsComunication
{
    public interface ITrainingUpdate
    {
        // Dopisuje wiadomość
        public void Update(string update);
        // Pobiera dopisane wiadomości
        public string ActualInfo(string login, string modelName);
        // Ustalamy dla kogo jest komunikat
        public void SetNewUserModel(string login, string modelName);
        // Sprawdza czy model danego użytkownika jest trenowany
        public bool IsUserModelInTraining(string login, string modelName);
        // sprawdza czy dany użytkownik trenuje 
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
            _infoMaker.Append(update);
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
