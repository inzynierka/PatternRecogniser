using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PatternRecogniser;
using PatternRecogniser.Controllers;
using PatternRecogniser.Models;
using PatternRecogniser.Services;
using PatternRecogniser.Services.Repos;
using PatternRecogniser.UnitsOfWorks;
using PatternRecogniserUnitTests.TestingInterfers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternRecogniserUnitTests.Controllers
{
    [TestClass]
    public class ExperimentListUnitTest
    {
        private Mock<IGenericRepository<ExperimentList>> _mockExperimentListRepo;
        private Mock<IGenericRepository<User>> _mockUserRepo;
        private Mock<IGenericRepository<ExtendedModel>> _mockExtendedModelRepo;
        private Mock<IGenericRepository<Experiment>> _mockExperimentRepo;
        private Mock<IGenericRepository<RecognisedPatterns>> _mockRecognisedPatternsRepo;
        private IExperimentListUnitOfWork _unitOfWorkTest;

        private ExperimentListController _controller;

        
       

        public ExperimentListUnitTest()
        {
            _mockExperimentListRepo = new Mock<IGenericRepository<ExperimentList>>().DefaultMockSetUp();
            _mockUserRepo = new Mock<IGenericRepository<User>>().DefaultMockSetUp();
            _mockExtendedModelRepo = new Mock<IGenericRepository<ExtendedModel>>().DefaultMockSetUp();
            _mockExperimentRepo = new Mock<IGenericRepository<Experiment>>().DefaultMockSetUp();
            _mockRecognisedPatternsRepo = new Mock<IGenericRepository<RecognisedPatterns>>().DefaultMockSetUp();
            _unitOfWorkTest = new ExperimentListUnitOfWorkTest(_mockExperimentListRepo.Object, _mockExtendedModelRepo.Object, _mockUserRepo.Object,
               _mockExperimentRepo.Object, _mockRecognisedPatternsRepo.Object);
            _controller = new ExperimentListController(
                _unitOfWorkTest);
        }

        [TestMethod]
        public void IsMockProperlyInit()
        {
            var cos =_unitOfWorkTest.experimentListRepo;
            Assert.IsNotNull(cos);
        }


        [TestMethod]
        public void Create_ListExsist()
        {
            var exsistedList = new ExperimentList()
            {
                userLogin = "test",
                name = "ListName",
                experimentType = "test"
            };
            var list = new List<ExperimentList>() {
                exsistedList
            };
            _mockExperimentListRepo.SetUpGet(list);

            var user = new User() { login = exsistedList.userLogin };
            _controller.SimulateAuthorizedUser(user);

            var respond = _controller.Create(exsistedList.name, exsistedList.experimentType);
            var objectResult = respond.Result as ObjectResult;
            Assert.AreEqual(objectResult.StatusCode, 400);
        }

        [TestMethod]
        public void Create_Succes()
        {
            _mockExperimentListRepo.SetUpGet(new List<ExperimentList>());
            var user = new User() { login = "ktoś" };
            _controller.SimulateAuthorizedUser(user);
            string listName = "CoolName";
            string listType = "CoolType";
            var respond = _controller.Create(listName, listType);
            var objectResult = respond.Result as OkResult;
            Assert.AreEqual(objectResult.StatusCode, 200);
        }

        [TestMethod]
        public void AddModelTrainingExperiment_CorrectInput()
        {
            var user = new User()
            {
                login = "CoolUser"
            };
            var ModelTrainingExperiment = new ModelTrainingExperiment();
            var model = new ExtendedModel()
            {
                extendedModelId = 1,
                name = "CoolModel",
                userLogin = user.login,
                modelTrainingExperiment = ModelTrainingExperiment
            };
            var list = new ExperimentList()
            {
                name = "CoolList",
                userLogin = user.login,
                experimentType = "ModelTrainingExperiment",
                experiments = new List<Experiment>()
            };

            _mockExtendedModelRepo.SetUpGet(new List<ExtendedModel>(){ model});
            _mockExperimentListRepo.SetUpGet(new List<ExperimentList>(){ list});
            _mockUserRepo.SetUpGet(new List<User>() { user });

            _controller.SimulateAuthorizedUser(user);
            var result = _controller.AddModelTrainingExperiment(list.name, model.extendedModelId).Result;
            var objectResult = result as OkObjectResult;

            Assert.AreEqual(objectResult.StatusCode, 200);
            Assert.AreEqual(list.experiments.ElementAt(0), ModelTrainingExperiment);

        }

       
    }
}
