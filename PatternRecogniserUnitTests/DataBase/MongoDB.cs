using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PatternRecogniser.Models;
using PatternRecogniser.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternRecogniserUnitTests.DataBase
{
    [TestClass]
    public class MongoDB
    {
        trainingInfoService trainingInfoService;
        public string TestedFiles { get; }
        private IFormFile _trainingSet;

        public MongoDB()
        {
            var conf = Helper.InitConfiguration();
            TrainingInfoSettings tis = new TrainingInfoSettings();
            conf.GetSection("TrainingInfoDBTest").Bind(tis);
            IOptions<TrainingInfoSettings> options = Options.Create<TrainingInfoSettings>(tis);
            trainingInfoService = new TrainingInfoMongoCollection(options);

            string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            TestedFiles = projectDirectory + "\\TestedFiles";
            string fileName = "cyfry.zip";
            string fileLocation = $"{TestedFiles}\\{fileName}";
            var file = File.OpenRead(fileLocation);
            _trainingSet = new FormFile(file, 0, file.Length, fileName, fileName);
        }
        [TestMethod]
        public void GetMongoData()
        {
            var itemki = trainingInfoService.GetAsync().Result;
            Assert.AreNotEqual(0, itemki.Count);
        }

        [TestMethod]
        public void AddData()
        {
            
            TrainingInfo info = new TrainingInfo("test", _trainingSet, "", PatternRecogniser.Models.DistributionType.TrainTest,
                80, 1);
            trainingInfoService.RemoveAsync(info.login).Wait();
            trainingInfoService.CreateAsync(info).Wait();
            var item = trainingInfoService.GetAsync(info.login).Result;
            Assert.AreNotEqual(info, item);
            trainingInfoService.RemoveAsync(info.login).Wait();
        }
    }
}
