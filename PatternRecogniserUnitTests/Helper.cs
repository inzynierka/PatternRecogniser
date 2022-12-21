using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using PatternRecogniser;
using PatternRecogniser.Models;
using PatternRecogniser.Services;
using PatternRecogniser.Services.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PatternRecogniserUnitTests
{
    public static class Helper
    {
        public static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();
            return config;
        }

        public static void SetUpGet<TEntity>(this Mock<IGenericRepository<TEntity>> mock, List<TEntity> mockData)
        {
            mock.Setup(a => a.Get(It.IsAny<Expression<Func<TEntity, bool>>>(), It.IsAny<string>()))
                .Returns((Expression<Func<TEntity, bool>> filter, string include) =>
                mockData.AsQueryable<TEntity>().Where(filter).ToList());
        }

        public static Mock<IGenericRepository<TEntity>> DefaultMockSetUp<TEntity>(this Mock<IGenericRepository<TEntity>> mock)
        {
            mock.Setup(a => a.Insert(It.IsAny<TEntity>())).Callback(() => { return; });
            mock.Setup(a => a.SaveChangesAsync()).Callback(() => { return; });
            mock.Setup(a => a.Delete(It.IsAny<TEntity>())).Callback(() => { return; });
            mock.Setup(a => a.Delete(It.IsAny<object>())).Callback(() => { return; });
            return mock;

        }

        public static T SimulateAuthorizedUser<T>(this T controller, User user) where T : ControllerBase
        {
            controller.EnsureHttpContext();
            controller.ControllerContext.HttpContext.User = CreateUserPrincipals(user);
            return controller;
        }

        private static ClaimsPrincipal CreateUserPrincipals(User user)
        {
            var conf = Helper.InitConfiguration();
            var authenticationSettings = new AuthenticationSettings();
            conf.GetSection("Authentication").Bind(authenticationSettings);
            var tokenCreator = new TokenCreator(authenticationSettings);
            var token = tokenCreator.CreateAccessToken(user);
            return tokenCreator.GetPrincipalFromExpiredToken(token);
        }

        private static T EnsureHttpContext<T>(this T controller) where T : ControllerBase
        {
            if (controller.ControllerContext == null)
            {
                controller.ControllerContext = new ControllerContext();
            }

            if (controller.ControllerContext.HttpContext == null)
            {
                controller.ControllerContext.HttpContext = new DefaultHttpContext();
            }

            return controller;
        }
    }
}
