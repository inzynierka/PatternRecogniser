using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using PatternRecogniser;
using PatternRecogniser.Models;
using PatternRecogniser.Services;
using PatternRecogniser.Services.Repos;
using System;
using System.Collections.Generic;
using System.IO;
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

        public static Mock<IGenericRepository<TEntity>> SetUpGet<TEntity>(this Mock<IGenericRepository<TEntity>> mock, List<TEntity> mockData)
        {
            mock.Setup(a => a.Get(It.IsAny<Expression<Func<TEntity, bool>>>(), It.IsAny<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>>()))
                .Returns((Expression<Func<TEntity, bool>> filter, string include) =>
                mockData.AsQueryable<TEntity>().Where(filter).ToList());
            return mock;
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

        public static TrainingInfo CreateSimpleTrainingInfo(string login, string modelName)
        {
            string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            var testedFiles = projectDirectory + "\\TestedFiles";
            string fileName = "cyfry.zip";
            string fileLocation = $"{testedFiles}\\{fileName}";
            var file = File.OpenRead(fileLocation);
            var trainingSet = new FormFile(file, 0, file.Length, fileName, fileName);

            return new TrainingInfo(login, trainingSet, modelName, PatternRecogniser.Models.DistributionType.TrainTest,
                80, 1);
        }

        public static TrainingInfoMongoCollection CreateTrainingInfoMongoCollection()
        {
            var conf = Helper.InitConfiguration();
            TrainingInfoSettings tis = new TrainingInfoSettings();
            conf.GetSection("TrainingInfoDBTest").Bind(tis);
            IOptions<TrainingInfoSettings> options = Options.Create<TrainingInfoSettings>(tis);
            return new TrainingInfoMongoCollection(options);
        }

        public static void  ClearTrainingInfoTestDB(this ItrainingInfoService trainingInfoService)
        {
            List<string> ids = trainingInfoService.GetAsync().Result.Select(a => a.id).ToList();
            foreach(var id in ids)
            {
                trainingInfoService.RemoveAsync(id);
            }
        }
    }
}
