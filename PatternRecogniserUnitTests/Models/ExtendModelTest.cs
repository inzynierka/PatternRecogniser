using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PatternRecogniser.Models;
using PatternRecogniser.ThreadsComunication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternRecogniserUnitTests.Models
{
    [TestClass]
    public  class ExtendModelTest
    {
        private IFormFile _trainingSet;

        public ExtendModelTest()
        {
            string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string TestedFiles = projectDirectory + "\\TestedFiles";
            string fileName = "cyfry.zip";
            string fileLocation = $"{TestedFiles}\\{fileName}";
            var file = File.OpenRead(fileLocation);
            _trainingSet = new FormFile(file, 0, file.Length, fileName, fileName);
        }

        [TestMethod]
        public void TrainingModelTest_TrainTest()
        {

            TrainingInfo info = new TrainingInfo("test", _trainingSet, "", PatternRecogniser.Models.DistributionType.TrainTest,
                80, 1);
            var model = new ExtendedModel();
            model.TrainModel(info.distributionType, null, info.trainingSet, info.trainingPercent, info.sets);
            Assert.IsNotNull(model.modelTrainingExperiment);
        }

        [TestMethod]
        public void TrainingModelTest_Cross()
        {
            TrainingInfo info = new TrainingInfo("test", _trainingSet, "", PatternRecogniser.Models.DistributionType.CrossValidation,
                80, 1);
            var model = new ExtendedModel();
            model.TrainModel(info.distributionType, null, info.trainingSet, info.trainingPercent, info.sets);
            Assert.IsNotNull(model.modelTrainingExperiment);
        }

    }
}
