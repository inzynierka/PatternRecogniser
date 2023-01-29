using PatternRecogniser.Models;
using PatternRecogniser.Services.Repos;
using PatternRecogniser.UnitsOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternRecogniserUnitTests.TestingInterfers
{
    internal class ExperimentListUnitOfWorkTest : IExperimentListUnitOfWork
    {

        private IGenericRepository<ExperimentList> _experimentListRepo;
        private IGenericRepository<ExtendedModel> _extendedModelRepo;
        private IGenericRepository<User> _userRepo;
        private IGenericRepository<Experiment> _experimentRepo;
        private IGenericRepository<RecognisedPatterns> _recognisedPatternsRepo;

        public ExperimentListUnitOfWorkTest(IGenericRepository<ExperimentList> experimentListRepo,
            IGenericRepository<ExtendedModel> extendedModelRepo,
            IGenericRepository<User> userRepo,
            IGenericRepository<Experiment> experimentRepo,
            IGenericRepository<RecognisedPatterns> recognisedPatternsRepo
            )
        {
            this._experimentListRepo = experimentListRepo;
            this._extendedModelRepo = extendedModelRepo;
            this._userRepo = userRepo;
            this._experimentRepo = experimentRepo;
            this._recognisedPatternsRepo = recognisedPatternsRepo;
        }

        public IGenericRepository<ExperimentList> experimentListRepo => _experimentListRepo;

        public IGenericRepository<ExtendedModel> extendedModelRepo => _extendedModelRepo;

        public IGenericRepository<User> userRepo => _userRepo;

        public IGenericRepository<Experiment> experimentRepo => _experimentRepo;

        public IGenericRepository<RecognisedPatterns> recognisedPatternsRepo => _recognisedPatternsRepo;

        public List<Experiment> GetAllExperiments(string experimentListName, string login)
        {
            throw new NotImplementedException();
        }

        public Task SaveChangesAsync()
        {
            return Task.CompletedTask;  
        }
    }
}
