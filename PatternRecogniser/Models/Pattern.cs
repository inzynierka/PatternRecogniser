using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

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

        public static int[,] ByteToInts(byte[] bytes, int rows = 28, int columns = 28)
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
    }
}
