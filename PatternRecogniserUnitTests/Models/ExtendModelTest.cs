using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PatternRecogniser.Messages.Model;
using PatternRecogniser.Models;
using PatternRecogniser.ThreadsComunication;
using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PatternRecogniserUnitTests.Models
{
    [TestClass]
    public class ExtendModelTest
    {
        private readonly ExtendedModelStringMessages messages = new ExtendedModelStringMessages();
        private IFormFile _trainingSet;
        private ITrainingUpdate _trainingUpdate;
        private CancellationTokenSource cancellationToken = new CancellationTokenSource();
        public string TestedFiles { get; }

        public ExtendModelTest()
        {
            string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            TestedFiles = projectDirectory + "\\TestedFiles";
            string fileName = "cyfry.zip";
            string fileLocation = $"{TestedFiles}\\{fileName}";
            var file = File.OpenRead(fileLocation);
            _trainingUpdate = new SimpleComunicationOneToMany();
            _trainingSet = new FormFile(file, 0, file.Length, fileName, fileName);
        }

        [TestMethod]
        public void TrainingModelTest_TrainTest()
        {

            TrainingInfo info = new TrainingInfo("test", _trainingSet, "", PatternRecogniser.Models.DistributionType.TrainTest,
                80, 1);
            var model = new ExtendedModel();
            model.TrainModel(info.distributionType, _trainingUpdate, info.trainingSet, info.trainingPercent, info.sets, cancellationToken.Token);
            Assert.IsNotNull(model.modelTrainingExperiment);
        }

        [TestMethod]
        public void RocCreationTest_TrainTest()
        {

            TrainingInfo info = new TrainingInfo("test", _trainingSet, "", PatternRecogniser.Models.DistributionType.TrainTest,
                80, 1);
            var model = new ExtendedModel();
            model.TrainModel(info.distributionType, _trainingUpdate, info.trainingSet, info.trainingPercent, info.sets, cancellationToken.Token);
            System.Diagnostics.Debug.WriteLine(model.modelTrainingExperiment.serializedRoc);
        }


        [TestMethod]
        public void TrainingModelTest_UpdateTest()
        {

            TrainingInfo info = new TrainingInfo("test", _trainingSet, "", PatternRecogniser.Models.DistributionType.TrainTest,
                80, 1);
            var model = new ExtendedModel();
            string login = "user";
            string modelName = "model";
            _trainingUpdate.SetNewUserModel(login, modelName);
            Task t =Task.Run(() => model.TrainModel(info.distributionType, _trainingUpdate, info.trainingSet, info.trainingPercent, info.sets, cancellationToken.Token), cancellationToken.Token);

            while (!t.IsCompleted)
            {
                System.Diagnostics.Debug.WriteLine(_trainingUpdate.ActualInfo(login, modelName));
                Task wait = Task.Delay(TimeSpan.FromSeconds(5));
                wait.Wait();
            }

            t.Wait();
            Assert.IsNotNull(model.modelTrainingExperiment);
            Assert.IsTrue(_trainingUpdate.ActualInfo(login, modelName).Contains(messages.startValidation));
        }

        [TestMethod]
        public void Loading_Saving_model()
        {

            TrainingInfo info = new TrainingInfo("test", _trainingSet, "", PatternRecogniser.Models.DistributionType.TrainTest,
                80, 1);
            var model = new ExtendedModel();
            model.TrainModel(info.distributionType, _trainingUpdate, info.trainingSet, info.trainingPercent, info.sets, cancellationToken.Token);
            Bitmap bitmap = new Bitmap(TestedFiles + "\\" + "0_0.png");
            var results = model.RecognisePattern(bitmap);
            Assert.IsNotNull(model.modelTrainingExperiment);
            Assert.AreEqual(results.Count, model.num_classes);
        }


        [TestMethod]
        public void TrainingModelTest_Cross()
        {
            TrainingInfo info = new TrainingInfo("test", _trainingSet, "", PatternRecogniser.Models.DistributionType.CrossValidation,
                80, 5);
            var model = new ExtendedModel();
            model.TrainModel(info.distributionType, _trainingUpdate, info.trainingSet, info.trainingPercent, info.sets, cancellationToken.Token);
            Assert.IsNotNull(model.modelTrainingExperiment);
        }

    }
}
