using Microsoft.VisualStudio.TestTools.UnitTesting;
using PatternRecogniser.Models;
using PatternRecogniser.Controllers;
using System.Linq;
using PatternRecogniser.Messages.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace PatternRecogniserUnitTests
{
    [TestClass]
    public class AuthenticationUnitTests
    {
        private PatternRecogniserDBContext _context;
        private AuthenticationController _authenticationController;

        public AuthenticationUnitTests()
        {
            _context = new TestDatabaseFixture().CreateContext();
            _authenticationController = new AuthenticationController(_context);
        }

        [TestMethod]
        public void SignUp()
        {
            string testedLogin = "testedLogin";
            string testedPassword = "password";
            string testedEmail = "email";

            SignUp signUpInfo = new SignUp()
            {
                email = testedEmail,
                login = testedLogin,
                password = testedPassword
            };

            _context.Database.BeginTransaction();

            var respond = _authenticationController.SignUp(signUpInfo);
            var result =  respond.Result;

            _context.ChangeTracker.Clear();

            var okResult = result as OkObjectResult;
            Assert.IsTrue(okResult.StatusCode == 200);
            var token = okResult.Value as Token;

            var addedUser = _context.user.Single(b => b.login == testedLogin);
            var authentication = _context.authentication.Single(b => b.userLogin == testedLogin);

            Assert.IsTrue(addedUser.login == testedLogin && addedUser.email == testedEmail && authentication.hashedToken == token.accessToken);
        }
    }
}
