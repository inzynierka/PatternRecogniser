using System.Text;

namespace PatternRecogniser.ThreadsComunication
{
    public interface ITrainingUpdate
    {
        // Dopisuje nowe info
        public void Update(string update);
        // Pobiera najnowsze info
        public string ActualInfo(int userId, string modelName);
        // Ustalamy dla kogo jest komunikat
        public void SetNewUserModel(int userId, string modelName);
    }

    public class SimpleComunicationOneToMany: ITrainingUpdate
    {
        private int  _userId { get; set; }
        private string _modelName;
        private StringBuilder _infoMaker = new StringBuilder();
        private string _infoPublisher;

        public string ActualInfo(int userId ,string modelName)
        {
            if(userId == this._userId)
                return _infoPublisher;
            else
                return string.Empty;
        }

        public void SetNewUserModel(int userId, string modelName)
        {
            // lokuje by nikt nie dostał nie swoje info, potencjalnie nie potrzebne
            lock (this) 
            { 
                _userId = userId;
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
    }
}
