using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PatternRecogniser;
using PatternRecogniser.Controllers;
using PatternRecogniser.Messages.Model;
using PatternRecogniser.Models;
using PatternRecogniser.Services;
using PatternRecogniser.Services.Repos;
using PatternRecogniser.ThreadsComunication;
using PatternRecogniserUnitTests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternRecogniserUnitTests
{
    [TestClass()]
    public class TrainModelControllerTests
    {
        private Mock<IHostedServiceDBConection> _mockHostedServiceDBConection;
        private Mock<IGenericRepository<User>> _mockUserRepo;
        private Mock<IGenericRepository<ExtendedModel>> _mockExtendedModelRepo;
        private Mock<IGenericRepository<Experiment>> _mockExperimentRepo;
        private FormFile _trainingSet;
        private readonly Mock<IGenericRepository<ModelTrainingExperiment>> _mockModelTrainingExperimentRepo;
        private readonly Mock<IGenericRepository<PatternRecognitionExperiment>> _mockPatternRecognitionExperimentRepo;
        private readonly ITrainingUpdate _trainingUpdate;
        private readonly TrainModelController _controller;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        private readonly TrainingModelQueuedHostedService _trainingModelQueuedHostedService;
        private readonly ExtendedModelStringMessages messages = new ExtendedModelStringMessages();
        public string TestedFiles { get; }

        public TrainModelControllerTests()
        {
            _mockUserRepo = new Mock<IGenericRepository<User>>().DefaultMockSetUp();
            _mockExtendedModelRepo = new Mock<IGenericRepository<ExtendedModel>>().DefaultMockSetUp();
            _mockExperimentRepo = new Mock<IGenericRepository<Experiment>>().DefaultMockSetUp();
            _mockModelTrainingExperimentRepo = new Mock<IGenericRepository<ModelTrainingExperiment>>().DefaultMockSetUp();
            _mockPatternRecognitionExperimentRepo = new Mock<IGenericRepository<PatternRecognitionExperiment>>().DefaultMockSetUp();

            _trainingUpdate = new SimpleComunicationOneToMany();
            var mongoDB = Helper.CreateTrainingInfoMongoCollection();
            mongoDB.ClearTrainingInfoTestDB();
            _backgroundTaskQueue = new BackgroundQueueLurchTable(mongoDB);
            _controller = new TrainModelController(
                _mockExtendedModelRepo.Object,
                _mockUserRepo.Object,
                _mockModelTrainingExperimentRepo.Object,
                _mockExperimentRepo.Object,
                _mockPatternRecognitionExperimentRepo.Object,
                _backgroundTaskQueue,
                _trainingUpdate
                );

            var settings = new TrainingSettings();
            settings.TimeoutInSeconds = 1800;
            _mockHostedServiceDBConection = new Mock<IHostedServiceDBConection>();
            _trainingModelQueuedHostedService = new TrainingModelQueuedHostedService(
                new Mock<ILogger<TrainingModelQueuedHostedService>>().Object,
                _backgroundTaskQueue,
                _mockHostedServiceDBConection.Object,
                _trainingUpdate,
                settings
                );
            string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            TestedFiles = projectDirectory + "\\TestedFiles";
            string fileName = "cyfry.zip";
            string fileLocation = $"{TestedFiles}\\{fileName}";
            var file = File.OpenRead(fileLocation);
            _trainingSet = new FormFile(file, 0, file.Length, fileName, fileName);
        }

        [TestMethod()]
        public void TrainModelControllerTest()
        {
            
        }

        [TestMethod()]
        public void TrainModelTest()
        {
            var user = new User()
            {
                login = "CoolUser"
            };
            List<ExtendedModel> captureModels = new List<ExtendedModel>();
            _mockUserRepo.SetUpGet(new List<User>() { user });
            _mockHostedServiceDBConection.Setup(m => m.GetUser(It.IsAny<string>())).Returns(user);
            _mockHostedServiceDBConection.Setup(m => m.SaveModel(Capture.In(captureModels))).Callback(() => { });

            string modelName = "modelName";
            _controller.SimulateAuthorizedUser(user);
            var result = _controller.TrainModel(modelName, 0, _trainingSet, 80, 4).Result;
            var objectResult = result as ObjectResult;
            Assert.AreEqual(objectResult.Value, 0);
            for (int i = 0; i < 4; i++)
            {
                result = _controller.NumberInQueue();
                objectResult = result as ObjectResult;
                if (i < 3)
                    Assert.AreEqual(objectResult.Value, 0);
                else
                {
                    Assert.IsInstanceOfType(objectResult.Value, typeof(string));

                }
                if (i == 2)
                {
                    _trainingModelQueuedHostedService.StartAsync(new System.Threading.CancellationToken());
                    while (_backgroundTaskQueue.Count > 0)
                    {
                        Task.Delay(TimeSpan.FromSeconds(2)).Wait();
                    }
                }
            }

            do
            {
                string actualInfoString = _trainingUpdate.ActualInfo(user.login, modelName);
                if(actualInfoString != string.Empty)
                {
                    Assert.IsTrue(actualInfoString.Contains(messages.startTraining));
                }
                System.Diagnostics.Debug.WriteLine(_trainingUpdate.ActualInfo(user.login, modelName));
                Task wait = Task.Delay(TimeSpan.FromSeconds(4));
                wait.Wait();
            } while (_trainingUpdate.IsUserTrainingModel(user.login));


            Assert.IsNotNull(captureModels[0].modelTrainingExperiment);
        }

        [TestMethod()]
        public void NumberInQueueTest()
        {
           
        }

        [TestMethod()]
        public void CancelTest()
        {
        }

        [TestMethod()]
        public void TrainUpdateTest()
        {
        }

        [TestMethod()]
        public void GetModelStatisticsTest()
        {
        }

        [TestMethod()]
        public void GetModelsTest()
        {
        }

        [TestMethod()]
        public void DeleteModelTest()
        {
        }

        [TestMethod()]
        public void GetModelStatusTest()
        {
        }
    }
}