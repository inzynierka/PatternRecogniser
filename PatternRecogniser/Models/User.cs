using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;
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
        public bool exsistUnsavePatternRecognitionExperiment { get; set; } = false;
        public string lastTrainModelName { get; set; }

        public DateTime createDate { get; set; }
        public DateTime lastLog { get; set; }

        private const int maxBitmapSize = 128; // do ustalenia

        public virtual ICollection<ExtendedModel> extendedModel { get; set; }
        public virtual ICollection<ExperimentList> experimentLists { get; set; }
        public virtual PatternRecognitionExperiment lastPatternRecognitionExperiment { get; set; }

        public void LoadTrainingSet(string path) 
        {
            // to jakoś w kontrolerze trzeba ogarnąć
            // na froncie otwieramy file explorer i wybieramy plik
            // przekazujemy tutaj ścieżkę do pliku
            // PatternData data = OpenZip(path);
            // tworzymy nowy ExtendedModel
            // wywołujemy w nim TrainModel (trzeba chyba by dodać parametr PatternData do tej metody)

            PatternData data = OpenZip (path);
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
                        string patternName = fullName.Substring(0, name.IndexOf('/'));

                        // obrazek patternu - byte array zawartości
                        Stream reader = entry.Open();
                        MemoryStream memstream = new MemoryStream();
                        reader.CopyTo(memstream);
                        byte[] array = memstream.ToArray();
                        MemoryStream ms = new MemoryStream(array);
                        Bitmap bmp = new Bitmap(ms);
                        int[,] matrix = NormaliseData(bmp);

                        // stwórz Pattern
                        Pattern pattern = new Pattern(patternName, matrix); // konstruktor zamienia int[,] na byte[]

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

        public void LoadTestingImage(string path) 
        {
            // też jakoś w kontrolerze
            // na froncie otwieramy file explorer i wybieramy plik
            // przekazujemy tutaj ścieżkę do pliku

            Bitmap img = OpenSingleImage (path);
            int[,] matrix = NormaliseData (img);

            // matrix możemy przekazać do RecognisePattern w ExtendedModel
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

        public int[,] NormaliseData(Bitmap bmp)
        {
            // jeden obrazek Bitmap
            bmp = new Bitmap (bmp, 28, 28);

            int height = bmp.Height; // 28
            int width = bmp.Width; // 28
            Bitmap contrastBmp = AdjustContrast(bmp, 100.0f);

            int[,] binaryMatrix = new int[height, width];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    // sprowadzamy kolor do skali szarości
                    Color pixelColor = contrastBmp.GetPixel(i, j);
                    int avg = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;

                    // odnaleziony kolor sprowadzamy do wartości 0/1
                    // normalnie 0 - czarny, 255 - biały
                    // zatem dzielimy przez 255 i zamieniamy
                    // wpisujemy do pola [j, i] żeby zapobiec obróceniu obrazka
                    float tmp = avg / 255;
                    if (tmp < 0.5) // czarny
                    {
                        binaryMatrix[j, i] = 1;
                    }
                    else // biały
                    {
                        binaryMatrix[j, i] = 0;
                    }
                }
            }

            return binaryMatrix;
        }

        private static Bitmap AdjustContrast(Bitmap Image, float Value) // https://stackoverflow.com/questions/3115076/adjust-the-contrast-of-an-image-in-c-sharp-efficiently
        {
            Value = (100.0f + Value) / 100.0f;
            Value *= Value;
            Bitmap NewBitmap = (Bitmap)Image.Clone ();
            BitmapData data = NewBitmap.LockBits (
                new Rectangle (0, 0, NewBitmap.Width, NewBitmap.Height),
                ImageLockMode.ReadWrite,
                NewBitmap.PixelFormat);
            int Height = NewBitmap.Height;
            int Width = NewBitmap.Width;

            unsafe // włączone "allow unsafe code"!!!
            {
                for (int y = 0; y < Height; ++y)
                {
                    byte* row = (byte*)data.Scan0 + (y * data.Stride);
                    int columnOffset = 0;
                    for (int x = 0; x < Width; ++x)
                    {
                        byte B = row[columnOffset];
                        byte G = row[columnOffset + 1];
                        byte R = row[columnOffset + 2];

                        float Red = R / 255.0f;
                        float Green = G / 255.0f;
                        float Blue = B / 255.0f;
                        Red = (((Red - 0.5f) * Value) + 0.5f) * 255.0f;
                        Green = (((Green - 0.5f) * Value) + 0.5f) * 255.0f;
                        Blue = (((Blue - 0.5f) * Value) + 0.5f) * 255.0f;

                        int iR = (int)Red;
                        iR = iR > 255 ? 255 : iR;
                        iR = iR < 0 ? 0 : iR;
                        int iG = (int)Green;
                        iG = iG > 255 ? 255 : iG;
                        iG = iG < 0 ? 0 : iG;
                        int iB = (int)Blue;
                        iB = iB > 255 ? 255 : iB;
                        iB = iB < 0 ? 0 : iB;

                        row[columnOffset] = (byte)iB;
                        row[columnOffset + 1] = (byte)iG;
                        row[columnOffset + 2] = (byte)iR;

                        columnOffset += 4;
                    }
                }
            }

            NewBitmap.UnlockBits (data);

            return NewBitmap;
        }

        public void SaveResult(Experiment experiment) { }

        public void SaveResultList(ExperimentList experimentList) { }

        public bool IsAbbleToAddPatternRecognitionExperiment()
        {
            return exsistUnsavePatternRecognitionExperiment && lastPatternRecognitionExperiment != null;
        }
    }
}
