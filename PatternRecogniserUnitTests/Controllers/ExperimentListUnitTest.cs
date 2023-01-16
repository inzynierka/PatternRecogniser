using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PatternRecogniser;
using PatternRecogniser.Controllers;
using PatternRecogniser.Models;
using PatternRecogniser.Services;
using PatternRecogniser.Services.Repos;
using PatternRecogniser.UnitsOfWorks;
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
        private Mock<IExperimentListUnitOfWork> _mockUnitOfWork;
        private ExperimentListController _controller;

        private IGenericRepository<TEntity> _mockExperimentListRepoCalls<TEntity>(List<TEntity> mockData = null)
        {
            if(mockData == null)
                return new Mock<IGenericRepository<TEntity>>().DefaultMockSetUp().Object;
            return new Mock<IGenericRepository<TEntity>>().DefaultMockSetUp().SetUpGet(mockData).Object;
        }
       

        public ExperimentListUnitTest()
        {
            _mockExperimentListRepo = new Mock<IGenericRepository<ExperimentList>>().DefaultMockSetUp();
            _mockUserRepo = new Mock<IGenericRepository<User>>().DefaultMockSetUp();
            _mockExtendedModelRepo = new Mock<IGenericRepository<ExtendedModel>>().DefaultMockSetUp();
            _mockExperimentRepo = new Mock<IGenericRepository<Experiment>>().DefaultMockSetUp();
            _mockRecognisedPatternsRepo = new Mock<IGenericRepository<RecognisedPatterns>>().DefaultMockSetUp();
            _mockUnitOfWork = new Mock<IExperimentListUnitOfWork>();
            _mockUnitOfWork.Setup(m => m.experimentListRepo).Callback(() => _mockExperimentListRepoCalls<ExperimentList>());
            _mockUnitOfWork.Setup(m => m.userRepo).Callback(() => _mockExperimentListRepoCalls<User>());
            _mockUnitOfWork.Setup(m => m.extendedModelRepo).Callback(() => _mockExperimentListRepoCalls<ExtendedModel>());
            _mockUnitOfWork.Setup(m => m.experimentRepo).Callback(() => _mockExperimentListRepoCalls<Experiment>());
            _mockUnitOfWork.Setup(m => m.recognisedPatternsRepo).Callback(() => _mockExperimentListRepoCalls<RecognisedPatterns>());
            
            _controller = new ExperimentListController(
                _mockUnitOfWork.Object);
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
            _mockUnitOfWork.Setup(m => m.experimentListRepo).Callback(() => _mockExperimentListRepoCalls<ExperimentList>(new List<ExperimentList>()));
            var cos = _mockUnitOfWork.Object;
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
            var result = _controller.AddModelTrainingExperiment(list.name, model.extendedModelId);
            var objectResult = result.Result as OkObjectResult;

            Assert.AreEqual(objectResult.StatusCode, 200);
            Assert.AreEqual(list.experiments.ElementAt(0), ModelTrainingExperiment);

        }

        [TestMethod]
        public void AddPatternRecognitionExperiment()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void GetLists_CorrectUser()
        {
            var user = new User()
            {
                login = "CoolLogin"
            };

            var listToGet = new ExperimentList()
            {
                name = "CoolNameForList",
                userLogin = user.login
            };
            var mockExperimentListData = new List<ExperimentList>()
            {
                listToGet,
                new ExperimentList()
                {
                    name = "NotCoolName",
                    userLogin = "NotCoolLogin"
                },
                new ExperimentList()
                {
                    name = "VeryUncoolName",
                    userLogin = "NotCoolLogin"
                }

            };

            _mockExperimentListRepo.SetUpGet(mockExperimentListData);
            _controller.SimulateAuthorizedUser(user);
            var respond = _controller.GetLists();
            var objectResult = respond as ObjectResult;
            Assert.AreEqual(listToGet, (objectResult.Value as List<ExperimentList>)[0]);
        }

        [TestMethod]
        public void GetExperiments()
        {
            var user = new User()
            {
                login = "CoolLogin"
            };

            var experimentModel = new ModelTrainingExperiment();
            var experimentPatternRecognition = new PatternRecognitionExperiment();

            var listToGetModelExperiments = new ExperimentList()
            {
                name = "CoolNameForList",
                userLogin = user.login,
                experiments = new List<Experiment>() { experimentModel }
            };
            var listToGetexperimentPatternRecognition = new ExperimentList()
            {
                name = "CoolNameForList2",
                userLogin = user.login,
                experiments = new List<Experiment>() { experimentPatternRecognition }
            };
            var mockExperimentListData = new List<ExperimentList>()
            {
                listToGetModelExperiments,
                listToGetexperimentPatternRecognition,
                new ExperimentList()
                {
                    name = "NotCoolName",
                    userLogin = "NotCoolLogin"
                },
                new ExperimentList()
                {
                    name = "VeryUncoolName",
                    userLogin = "NotCoolLogin"
                }

            };


            _mockExperimentListRepo.SetUpGet(mockExperimentListData);
            _controller.SimulateAuthorizedUser(user);
            var respond = _controller.GetExperiments(listToGetexperimentPatternRecognition.name);
            var objectResult = respond as ObjectResult;
            Assert.AreEqual(listToGetexperimentPatternRecognition.experiments.ElementAt(0), (objectResult.Value as List<Experiment>)[0]);

            respond = _controller.GetExperiments(listToGetModelExperiments.name);
            objectResult = respond as ObjectResult;
            Assert.AreEqual(listToGetModelExperiments.experiments.ElementAt(0), (objectResult.Value as List<Experiment>)[0]);

        }

        [TestMethod]
        public void DeleteList()
        {

        }
    }
}
