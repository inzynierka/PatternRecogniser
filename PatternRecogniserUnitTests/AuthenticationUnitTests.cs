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
using PatternRecogniserUnitTests.TestingSets;
using System.Collections.Generic;
using PatternRecogniser.Messages.Validators;
using PatternRecogniser.Services.NewFolder;
using PatternRecogniser.Services.Repos;
using System.Linq.Expressions;
using System;

namespace PatternRecogniserUnitTests
{
    [TestClass]
    public class AuthenticationUnitTests
    {
        private SignUp _signUpInfo;
        private LogIn _logInInfo;
        private Mock<IGenericRepository<User>> _mockRepo;
        private IAuthenticationServicis _authenticationServicis;
        private IPasswordHasher<User> _passwordHasher;
        private readonly ITokenCreator _tokenCreator;
        private readonly AuthenticationStringMesseges _messeges = new AuthenticationStringMesseges();

        public AuthenticationUnitTests()
        {
            var conf = Helpers.InitConfiguration();
            var authenticationSettings = new AuthenticationSettings();
            conf.GetSection("Authentication").Bind(authenticationSettings);


            _passwordHasher = new PasswordHasher<User>();
            _tokenCreator = new TokenCreator(authenticationSettings);
            _authenticationServicis = new AuthenticationServicis(_passwordHasher, _tokenCreator);
            _mockRepo = new Mock<IGenericRepository<User>>();

            _signUpInfo = new SignUp()
            {
                email = "tested@emial.com",
                login = "testedLogin",
                password = "testedPassword"
            };
            _logInInfo = new LogIn()
            {
                login = "test",
                password = "password"
            };
        }

        [TestMethod]
        public void SignUp_LoginAndEmeilTaken()
        {
            SetUpMock(new List<User>() { 
                new User() { email = _signUpInfo.email }, 
                new User() { login = _signUpInfo.login } });
            var _validator = new AuthentycationValidatorSingUp(_mockRepo.Object);
            var result = _validator.Validate(_signUpInfo);
            Assert.AreEqual(result.Errors.Count, 2);

        }

        [TestMethod]
        public void SignUp_ReturnToken()
        {
            SetUpMock(new List<User>());
            var controller = new AuthenticationController(_authenticationServicis, _mockRepo.Object);
            var respond = controller.SignUp(_signUpInfo);
            var okResult = respond.Result as OkObjectResult;
            var tokens = okResult.Value as Tokens;

            Assert.IsNotNull(tokens.accessToken);
            Assert.IsNotNull(tokens.refreshToken);
            Assert.AreEqual(okResult.StatusCode, 200);
        }


        [TestMethod]
        public void LogIn_IncorectPassword()
        {
            SetUpMock(new List<User>() { new User()
            {
                login = _logInInfo.login,
                hashedPassword = _passwordHasher.HashPassword(new User(), _logInInfo.password + "twist")
                } });

            var _validator = new AuthentycationValidatorLogIn(_mockRepo.Object, _passwordHasher);
            var result = _validator.Validate(_logInInfo);

            Assert.AreEqual(result.Errors.Where(a => a.PropertyName == "password").First().ErrorMessage,
                _messeges.incorectPassword);
        }

        [TestMethod]
        public void LogIn_ReturnCorrectDateWhenSucced()
        {
            var user = new User()
            {
                login = _logInInfo.login,
                email = "emailTestowy",
                hashedPassword = _passwordHasher.HashPassword(new User(), _logInInfo.password)
            };
            SetUpMock(new List<User>() { user });

            var controller = new AuthenticationController(_authenticationServicis, _mockRepo.Object);
            var respond = controller.LogIn(_logInInfo);
            var okResult = respond.Result as OkObjectResult;
            var body = okResult.Value as LogInRespond;
            var tokens = body.tokens;

            Assert.AreEqual(okResult.StatusCode, 200);
            Assert.AreEqual(user.email, body.email);
            Assert.IsNotNull(tokens.accessToken);
            Assert.IsNotNull(tokens.refreshToken);
        }
        private void SetUpMock(List<User> mockData)
        {
            _mockRepo.Setup(a => a.Get(It.IsAny<Expression<Func<User, bool>>> (), It.IsAny<string>()))
                .Returns( (Expression<Func<User, bool>> filter, string include) => 
                mockData.AsQueryable<User>().Where(filter).ToList() );
        }
    }
}
