using Microsoft.EntityFrameworkCore;
using PatternRecogniser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PatternRecogniser.Services.Repos
{
    public interface IGenericRepository<TEntity>
    {
        public void Insert(TEntity entity);
        public Task SaveChangesAsync();
        public List<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "");
        public void Delete(object id);
        public void Delete(TEntity entityToDelete);
        public void Update(TEntity entityToUpdate);
        public TEntity GetByID(object id);
    }


    public class GenericRepository<TEntity>: IGenericRepository<TEntity> where TEntity : class
    {
        private PatternRecogniserDBContext _context;
        private DbSet<TEntity> _dbSet;

        public GenericRepository(PatternRecogniserDBContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public void Insert(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public List<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            return query.ToList();
        }

        public virtual void Delete(object id)
        {
            TEntity entityToDelete = _dbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (_context.Entry(entityToDelete).State == EntityState.Detached)
            {
                _dbSet.Attach(entityToDelete);
            }
            _dbSet.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            _dbSet.Attach(entityToUpdate);
            _context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public virtual TEntity GetByID(object id)
        {
            return _dbSet.Find(id);
        }
    }
}
