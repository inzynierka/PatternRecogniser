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
        }
    }
}
