using Microsoft.EntityFrameworkCore;
using PatternRecogniser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PatternRecogniser.Services.Repos
{
    public class ExperimentListRepository
    {
        private PatternRecogniserDBContext _context;
        private DbSet<ExperimentList> _dbSet;

        public ExperimentListRepository(PatternRecogniserDBContext context)
        {
            _context = context;
            _dbSet = context.Set<ExperimentList>();
        }

        public async Task AddUserToDB(ExperimentList experimentList)
        {
            _context.experimentList.Add(experimentList);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }


        public List<ExperimentList> GetLists(Expression<Func<ExperimentList, bool>> filter = null)
        {
            IQueryable<ExperimentList> query = _dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return query.ToList();
        }

        public List<Experiment> GetExperimentsFromListList(Expression<Func<ExperimentList, bool>> filter = null)
        {
            IQueryable<ExperimentList> query = _dbSet.Include(a => a.experiments).ThenInclude( b => b.extendedModel);
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return query.First().experiments.ToList();
        }
    }
}
