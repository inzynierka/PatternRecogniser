using Microsoft.EntityFrameworkCore;
using PatternRecogniser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternRecogniserUnitTests
{
    public class TestDatabaseFixture
    {
        // używam kopi bazy danych na lokalnym serwerze, railway nie pozwala mi zrobić 2 daz :(
        private const string ConnectionString = "User ID=postgres;Password=1234;Host=localhost;Port=5432;Database=TestDatabase;Pooling=true;";


        private static readonly object _lock = new();
        private static bool _databaseInitialized;

        public TestDatabaseFixture()
        {
            lock (_lock)
            {
                if (!_databaseInitialized)
                {
                    using (var context = CreateContext())
                    {
                        context.Database.EnsureDeleted();
                        context.Database.EnsureCreated();

                        var user = new User { login = "InitTestUser", email = "email@test1.com" };
                        var extendModel = new ExtendedModel { name = "initTestedModel" };
                        var statistics = new ModelTrainingExperiment() { extendedModel = extendModel };
                        extendModel.modelTrainingExperiment = statistics;
                        user.extendedModel = new List<ExtendedModel> { extendModel };
                        context.Add(user);
                        context.SaveChanges();
                    }

                    _databaseInitialized = true;
                }
            }
        }

        public PatternRecogniserDBContext CreateContext()
            => new PatternRecogniserDBContext(
                new DbContextOptionsBuilder<PatternRecogniserDBContext>()
                    .UseNpgsql(ConnectionString)
                    .Options);
    }
}
