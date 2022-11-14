using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace PatternRecogniser.Models
{
    public class Pattern
    {
        public string name { get; set; }
        public Bitmap picture { get; set; }
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
            foreach(List<Pattern> list in patterns)
            {
                if(list[0].name == pattern.name)
                {
                    list.Add(pattern);
                    return;
                }
            }
        }
    }
}
