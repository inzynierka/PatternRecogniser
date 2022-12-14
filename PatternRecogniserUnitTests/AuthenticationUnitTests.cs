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

namespace PatternRecogniserUnitTests
{
    [TestClass]
    public class AuthenticationUnitTests
    {
        private PatternRecogniserDBContext _context;
        private AuthenticationController _authenticationController;
        private IConfiguration _conf;
        private IPasswordHasher<User> _passwordHasher;
        private User _testedUser;
        private Mock<IAuthenticationServicis> _mockRepo;
        private AuthenticationController _controller;
        private string _testedPassword;
        private string connectionString = "User ID=postgres;Password=AdJjYAmyKx5pXCDN7qhH;Host=containers-us-west-97.railway.app;Port=7363;Database=railway;Pooling=true;";

        public AuthenticationUnitTests()
        {
            _mockRepo = new Mock<IAuthenticationServicis>();
            _controller = new AuthenticationController(_mockRepo.Object);
            _testedPassword = "password";
            _testedUser = new User()
            {
                login = "testedUser",
                email = "testedEmail",
                hashedPassword = _passwordHasher.HashPassword(new User(), _testedPassword)
            };
        }

        [TestMethod]
        public void SignUp()
        {


            SignUp signUpInfo = new SignUp()
            {
                email = _testedUser.email,
                login = _testedUser.login,
                password = _testedPassword
            };

            var dbContext = new PatternRecogniserDBContext(null) ;
            _mockRepo.Setup(repo => repo.SignUp(signUpInfo));


            //_context.Database.BeginTransaction();

            //var user = _context.user.FirstOrDefault(b => b.login == _testedUser.login);
            //if (user != null)
            //    _context.user.Remove(user);
            //_context.SaveChanges();

            //var respond = _authenticationController.SignUp(signUpInfo);
            //var result = respond.Result;
            //_context.ChangeTracker.Clear();


            //var okResult = result as OkObjectResult;
            //Assert.IsTrue(okResult.StatusCode == 200);
            //var token = okResult.Value as Tokens;
            //var addedUser = _context.user.Single(b => b.login == _testedUser.login);

            //Assert.IsTrue(_passwordHasher.VerifyHashedPassword(addedUser, addedUser.refreshToken, token.refreshToken) != PasswordVerificationResult.Failed);
        }


        [TestMethod]
        public void LogIn()
        {


            LogIn logInInfo = new LogIn()
            {
                login = _testedUser.login,
                password = _testedPassword
            };


            _context.Database.BeginTransaction();

            var user = _context.user.FirstOrDefault(b => b.login == _testedUser.login);
            if (user != null)
                _context.user.Remove(user);
            _context.user.Add(_testedUser);
            _context.SaveChanges();

            var respond = _authenticationController.LogIn(logInInfo);
            var result = respond.Result;
            _context.ChangeTracker.Clear();


            var okResult = result as OkObjectResult;

            Assert.IsTrue(okResult.StatusCode == 200);
        }
    }
}
