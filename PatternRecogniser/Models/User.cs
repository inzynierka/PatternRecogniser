using Microsoft.EntityFrameworkCore;
using PatternRecogniser.Messages.TrainModel;
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
        public string hashedPassword { get; set; }
        public string lastTrainModelName { get; set; }
        public string lastCheckModel { get; set; }
        public ModelStatus lastModelStatus { get; set; }
        public int? lastPatternRecognitionExperimentexperimentId { get; set; }
        public DateTime createDate { get; set; }
        public DateTime lastLog { get; set; }
        public string refreshToken { get; set; }
        public DateTime refreshTokenExpiryDate { get; set; }

        private const int maxBitmapSize = 128; 
        

        public virtual ICollection<ExtendedModel> extendedModel { get; set; }
        public virtual ICollection<ExperimentList> experimentLists { get; set; }
        public virtual PatternRecognitionExperiment lastPatternRecognitionExperiment { get; set; }



        public bool IsAbbleToAddPatternRecognitionExperiment()
        {
            return lastPatternRecognitionExperiment != null;
        }
    }
}
