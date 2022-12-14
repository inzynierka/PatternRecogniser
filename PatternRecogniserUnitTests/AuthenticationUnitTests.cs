using Microsoft.VisualStudio.TestTools.UnitTesting;
using PatternRecogniser.Models;
using PatternRecogniser.Controllers;
using System.Linq;
using PatternRecogniser.Messages.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PatternRecogniser;
using Microsoft.AspNetCore.Identity;
using PatternRecogniser.Services;
using Moq;
using PatternRecogniser.Models.Validators;
using FluentValidation.TestHelper;
using FluentValidation;
using FluentValidation.TestHelper;

namespace PatternRecogniserUnitTests
{
    [TestClass]
    public class AuthenticationUnitTests
    {
        private SignUp signUpInfo;
        private Mock<IAuthenticationServicis> _mockRepo;
        private string _testedPassword;
        public AuthenticationUnitTests()
        {
            
            _mockRepo = new Mock<IAuthenticationServicis>();
            
            signUpInfo = new SignUp()
            {
                email = "tested@emial.com",
                login = "testedLogin",
                password = "testedPassword"
            };
        }

        public void InputDataValidatorTest()
        {
            _mockRepo.Setup(repo => repo.IsLoginTaken(signUpInfo.login))
        .Returns(true);
            _mockRepo.Setup(repo => repo.IsEmailTaken(signUpInfo.email))
        .Returns(true);
            var _validator = new AuthentycationValidatorSingUp(_mockRepo.Object);
            var result = _validator.Validate(signUpInfo);

            //result.ShouldHaveValidationErrorFor

        }

        [TestMethod]
        public void SignUp_DateValidationTest()
        {

            _mockRepo.Setup(repo => repo.IsLoginTaken(signUpInfo.login))
        .Returns(true);

            var controller = new AuthenticationController(_mockRepo.Object);
            var respond = controller.SignUp(signUpInfo);

         }


        [TestMethod]
        public void LogIn()
        {


            //LogIn logInInfo = new LogIn()
            //{
            //    login = _testedUser.login,
            //    password = _testedPassword
            //};


            //_context.Database.BeginTransaction();

            //var user = _context.user.FirstOrDefault(b => b.login == _testedUser.login);
            //if (user != null)
            //    _context.user.Remove(user);
            //_context.user.Add(_testedUser);
            //_context.SaveChanges();

            //var respond = _authenticationController.LogIn(logInInfo);
            //var result = respond.Result;
            //_context.ChangeTracker.Clear();


            //var okResult = result as OkObjectResult;

            //Assert.IsTrue(okResult.StatusCode == 200);
        }
    }
}
