using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PatternRecogniser.Controllers;
using PatternRecogniser.Models;
using PatternRecogniser.Services.Repos;
using PatternRecogniser.ThreadsComunication;
using PatternRecogniserUnitTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternRecogniserUnitTests
{
    [TestClass()]
    public class TrainModelControllerTests
    {
        private Mock<IGenericRepository<User>> _mockUserRepo;
        private Mock<IGenericRepository<ExtendedModel>> _mockExtendedModelRepo;
        private Mock<IGenericRepository<Experiment>> _mockExperimentRepo;
        private readonly Mock<IGenericRepository<ModelTrainingExperiment>> _mockModelTrainingExperimentRepo;
        private readonly Mock<IGenericRepository<PatternRecognitionExperiment>> _mockPatternRecognitionExperimentRepo;
        private readonly ITrainingUpdate _trainingUpdate;
        private readonly TrainModelController _controller;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;

        public TrainModelControllerTests()
        {
            _mockUserRepo = new Mock<IGenericRepository<User>>().DefaultMockSetUp();
            _mockExtendedModelRepo = new Mock<IGenericRepository<ExtendedModel>>().DefaultMockSetUp();
            _mockExperimentRepo = new Mock<IGenericRepository<Experiment>>().DefaultMockSetUp();
            _mockModelTrainingExperimentRepo = new Mock<IGenericRepository<ModelTrainingExperiment>>().DefaultMockSetUp();
            _mockPatternRecognitionExperimentRepo = new Mock<IGenericRepository<PatternRecognitionExperiment>>().DefaultMockSetUp();

            _trainingUpdate = new SimpleComunicationOneToMany();
            _backgroundTaskQueue = new BackgroundQueueLurchTable(Helper.CreateTrainingInfoMongoCollection());
            _controller = new TrainModelController(
                _mockExtendedModelRepo.Object,
                _mockUserRepo.Object,
                _mockModelTrainingExperimentRepo.Object,
                _mockExperimentRepo.Object,
                _mockPatternRecognitionExperimentRepo.Object,
                _backgroundTaskQueue,
                _trainingUpdate
                );
        }

        [TestMethod()]
        public void TrainModelControllerTest()
        {
            
        }

        [TestMethod()]
        public void TrainModelTest()
        {
            
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