using Microsoft.EntityFrameworkCore;
using PatternRecogniser.Models;
using PatternRecogniser.Services.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PatternRecogniser.UnitsOfWorks
{
    public interface IExperimentListUnitOfWork
    {
        public IGenericRepository<ExperimentList> experimentListRepo { get; }
        public IGenericRepository<ExtendedModel> extendedModelRepo { get; }
        public IGenericRepository<User> userRepo { get; }
        public IGenericRepository<Experiment> experimentRepo { get; }
        public IGenericRepository<RecognisedPatterns> recognisedPatternsRepo { get; }
        public List<Experiment> GetAllExperiments(string experimentListName, string login);
        Task SaveChangesAsync();    
    }
    public class ExperimentListUnitOfWork:IExperimentListUnitOfWork
    {
        private PatternRecogniserDBContext _context;
        private IGenericRepository<ExperimentList> _experimentListRepo;
        private IGenericRepository<ExtendedModel> _extendedModelRepo;
        private IGenericRepository<User> _userRepo;
        private IGenericRepository<Experiment> _experimentRepo;
        private IGenericRepository<RecognisedPatterns> _recognisedPatternsRepo;


        public ExperimentListUnitOfWork(PatternRecogniserDBContext context)
        {
            _context = context;
        }

        public IGenericRepository<ExperimentList> experimentListRepo { 
            get
            {
                if (_experimentListRepo == null)
                    _experimentListRepo = new GenericRepository<ExperimentList>(_context);
                return _experimentListRepo;
            }
        }

        public IGenericRepository<ExtendedModel> extendedModelRepo
        {
            get
            {
                if (_extendedModelRepo == null)
                    _extendedModelRepo = new GenericRepository<ExtendedModel>(_context);
                return _extendedModelRepo;
            }
        }

        public IGenericRepository<User> userRepo  {
            get
            {
                if (_userRepo == null)
                    _userRepo = new GenericRepository<User>(_context);
                return _userRepo;
            }
}

public IGenericRepository<Experiment> experimentRepo
        {
            get
            {
                if (_experimentRepo == null)
                    _experimentRepo = new GenericRepository<Experiment>(_context);
                return _experimentRepo;
            }
        }

        public IGenericRepository<RecognisedPatterns> recognisedPatternsRepo
        {
            get
            {
                if (_recognisedPatternsRepo == null)
                    _recognisedPatternsRepo = new GenericRepository<RecognisedPatterns>(_context);
                return _recognisedPatternsRepo;
            }
        }

        public List<Experiment> GetAllExperiments(string experimentListName, string login)
        {
            var query = GetAllExperimentsQuery(experimentListName, login);
            var returnData = query.ToList();
            // include rzeczy z patternRecogitionExperiment, nie wiem czemu się łączą ale się łączą
            var paternsEx = query.Join(
                    _context.patternRecognitionExperiment.Include(patternRecognitionExperiment => patternRecognitionExperiment.recognisedPatterns),
                    experiment => experiment.experimentId,
                    patternRecognitionExperiment => patternRecognitionExperiment.experimentId,
                    (experiment, patternRecognitionExperiment) => patternRecognitionExperiment)
                .ToList();
            return returnData;
        }

        private IQueryable<Experiment> GetAllExperimentsQuery(string experimentListName, string login)
        {
            return _context.experimentList
                .Where(a => a.name == experimentListName && a.userLogin == login)
                .Include(experimentList => experimentList.experiments)
                .SelectMany(experimentList => experimentList.experiments, (experimentList, experiment) => experiment);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}
