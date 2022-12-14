
//using NumSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Tensorflow;
using Tensorflow.NumPy;


namespace PatternRecogniser.Models
{
    public class Pattern
    {
        [Key]
        public int patternId { get; set; }
        public string name { get; set; }
        public byte[] picture { get; set; }
        public int extendedModelId { get; set; }

        public virtual ExtendedModel extendedModel { get; set; }

        public Pattern(string name, byte[] picture)
        {
            this.name = name;
            this.picture = picture;
        }
        public Pattern() { }

        public Pattern(string name, int[,] matrix)
        {
            this.name = name;
            this.picture = IntsToByte(matrix);
        }

        public static byte[] IntsToByte(int[,] matrix)
        {
            int size = matrix.GetLength(0) * matrix.GetLength(1);
            byte[] result = new byte[size];

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    result[i * matrix.GetLength(1) + j] = (byte)matrix[i, j];
                }
            }

            return result;
        }

        public int[,] ByteToInts(byte[] bytes, int rows, int columns)
        {
            int[,] result = new int[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    result[i, j] = bytes[i * columns + j];
                }
            }

            return result;
        }
    }

    public class PatternData 
    {
        public List<List<Pattern>> patterns;

        public void AddPatterns(List<Pattern> list)
        {
            patterns.Add(list);
        }

        public void AddPattern(Pattern pattern)
        {
            foreach (List<Pattern> list in patterns)
            {
                if (list[0].name == pattern.name)
                {
                    list.Add(pattern);
                    return;
                }
            }

            List<Pattern> newList = new List<Pattern>();
            newList.Add(pattern);
            patterns.Add(newList);
        }

        public int GetNumberOfClasses()
        {
            return patterns.Count;
        }

        public (Tensor, Tensor) PatternToTensor() // zwraca (obrazki, etykiety)
        {
            Tensor x_arr, y_arr;
            int k = 0; // numeryczne klasy, trzeba by zrobić jakąś konwersję/połączenie
            List<float[]> pictures = new List<float[]> ();
            List<int> classes = new List<int> ();
            foreach(List<Pattern> list in patterns)
            {
                foreach(Pattern pattern in list)
                {
                    float[] arr = new float[pattern.picture.Length];
                    for (int i = 0; i < pattern.picture.Length; i++)
                    {
                        arr[i] = pattern.picture[i];
                    }
                    pictures.Add(arr);
                    classes.Add (k);
                }
                k++; // klasę zmieniamy gdy skończymy jedną listę 
            }
            x_arr = ops.convert_to_tensor (pictures.ToArray (), TF_DataType.TF_FLOAT);
            y_arr = ops.convert_to_tensor(classes.ToArray(), TF_DataType.TF_INT32);

            return (x_arr, y_arr);
        }
    }
}
