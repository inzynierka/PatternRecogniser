using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.IO;

namespace PatternRecogniser.Models
{
    [BsonIgnoreExtraElements]
    public class TrainingInfo
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public string login { get; set; }
        public byte[] trainingSet { get; set; }
        public string modelName { get; set; }
        public DistributionType distributionType { get; set; }
        public int trainingPercent { get; set; }
        public int sets { get; set; }
        public DateTime addedTime {get; set;}

        
        public TrainingInfo(string login, byte[] trainingSet, string modelName, DistributionType distributionType, int trainingPercent, int sets)
        {
            this.login = login;
            this.trainingSet = trainingSet;
            this.modelName = modelName;
            this.distributionType = distributionType;
            this.trainingPercent = trainingPercent;
            this.sets = sets;
            addedTime = DateTime.Now;
        }
        public TrainingInfo(string login, IFormFile trainingSet, string modelName, DistributionType distributionType, int trainingPercent, int sets)
        {
            this.login = login;
            this.trainingSet = ReadFully(trainingSet.OpenReadStream());
            this.modelName = modelName;
            this.distributionType = distributionType;
            this.trainingPercent = trainingPercent;
            this.sets = sets;
            addedTime = DateTime.Now;

        }





        private byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }



    }
}
