using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace PatternRecogniser.Models
{
    [Index(nameof(email), IsUnique = true)]
    public class User
    {
        [Key]
        public string login { get; set; }
        [Required]
        public string email { get; set; }
        public string hashedPassword { get; set; }
        public bool exsistUnsavePatternRecognitionExperiment { get; set; } = false;
        public string lastTrainModelName { get; set; }
        public DateTime createDate { get; set; }
        public DateTime lastLog { get; set; }
        public string refreshToken { get; set; }
        public DateTime refreshTokenExpiryDate { get; set; }

        private const int maxBitmapSize = 128; // do ustalenia
        

        public virtual ICollection<ExtendedModel> extendedModel { get; set; }
        public virtual ICollection<ExperimentList> experimentLists { get; set; }
        public virtual PatternRecognitionExperiment lastPatternRecognitionExperiment { get; set; }

        public void LoadTrainingSet() 
        { 
            // to jakoś w kontrolerze trzeba ogarnąć
            // na froncie otwieramy file explorer i wybieramy plik
            // przekazujemy tutaj ścieżkę do pliku
            // PatternData data = OpenZip(path);
            // tworzymy nowy ExtendedModel
            // wywołujemy w nim TrainModel (trzeba chyba by dodać parametr PatternData do tej metody)
        }

        private PatternData OpenZip(string path)
        {
            PatternData data = new PatternData();
            if (CheckZipStructure(path) == false)
                return data; // zwraca puste dane, potem użytkownikowi mówimy że coś nie halo

            data.patterns = new List<List<Pattern>>();
            using (ZipArchive zip = ZipFile.OpenRead(path))
            {
                foreach (ZipArchiveEntry entry in zip.Entries)
                {
                    string fullName = entry.FullName;
                    string name = entry.Name;
                    if (name.Length > 0) // plik nie folder
                    {
                        // etykieta pattern - nazwa folderu - FullName do /
                        string patternName = name.Substring(0, name.IndexOf('/'));

                        // obrazek patternu - byte array zawartości
                        byte[] array = new byte[maxBitmapSize];
                        using (Stream stream = entry.Open())
                        {
                            int read = stream.Read(array, 0, maxBitmapSize);
                            if (read <= 0)
                                return new PatternData(); // był jakiś błąd, nie odczytaliśmy poprawnie pliku
                        }

                        // stwórz Pattern
                        Pattern pattern = new Pattern(patternName, array);

                        // dodaj do PatternData
                        data.AddPattern(pattern);
                    }
                }
            }

            return data;
        }

        private bool CheckZipStructure(string path)
        {
            // sprawdź czy to w ogóle zip (albo tylko zip będziemy wyświetlać, idk)
            FileInfo fi = new FileInfo(path);
            if (!fi.Extension.Equals(".zip"))
                return false;

            // otwieramy zip i sprawdzamy strukturę
            using (ZipArchive zip = ZipFile.OpenRead(path))
            {
                foreach (ZipArchiveEntry entry in zip.Entries)
                {
                    string tmp = entry.FullName;
                    int ind = tmp.IndexOf('/');

                    // gdyby Substring==0 to byłby to folder
                    if (ind > 0 && tmp.Substring(ind + 1).Length != 0)
                    {
                        // plik w folderze
                        // sprawdzamy czy nie ma podfolderów
                        if (tmp.Substring(ind + 1).IndexOf('/') >= 0)
                        {
                            return false;
                        }
                    }
                    else if (ind < 0) // plik poza folderem
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public void LoadTestingImage() 
        { 
            // też jakoś w kontrolerze
            // na froncie otwieramy file explorer i wybieramy plik
            // przekazujemy tutaj ścieżkę do pliku
            // Bitmap img = OpenSingleImage(path);
            // wywołujemy RecognisePattern z wybranego ExtendedModel
        }

        private Bitmap OpenSingleImage(string path)
        {
            Bitmap bmp = new Bitmap(path);

            return bmp;
        }

        public void StartModelTraining() { }

        public void StartRecognising() { }

        public void CreateExperimentList() { }

        public void AddExperiment(Experiment experiment, ExperimentList experimentList)
        {
            foreach (ExperimentList list in experimentLists)
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

        public bool IsAbbleToAddPatternRecognitionExperiment()
        {
            return exsistUnsavePatternRecognitionExperiment && lastPatternRecognitionExperiment != null;
        }
    }
}
