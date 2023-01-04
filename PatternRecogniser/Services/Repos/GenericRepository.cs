using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
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
        public List<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null);
        public List<TResult> Get<TResult>(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            Expression<Func<TEntity, TResult>> selector = null);

        public List<TResult> GetSelectMany<TCollection, TResult>(Expression<Func<TEntity, IEnumerable<TCollection>>> selectMany,
            Expression<Func<TEntity, TCollection, TResult>> resultSelector,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null);

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
            _context.experimentList.SelectMany(a => a.experiments, (el, ex) => new
            {
                extendModel = new
                {
                    ex.extendedModel.extendedModelId,
                    ex.extendedModel.name,
                    ex.extendedModel.userLogin,
                    ex.extendedModel.distribution,
                    ex.extendedModel.num_classes

                }
            });
        }

        public void Insert(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public List<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, 
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (include != null)
            {
                query = include(query);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            

            return query.ToList();
        }


        public List<TResult> Get<TResult>(Expression<Func<TEntity, bool>> filter = null,
           Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
           Expression<Func<TEntity, TResult>> selector = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (include != null)
            {
                query = include(query);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }



            return query.Select(selector).ToList();
        }

        public List<TResult> GetSelectMany<TCollection, TResult>(Expression<Func<TEntity, IEnumerable<TCollection>>> selectMany,
            Expression<Func<TEntity, TCollection, TResult>> resultSelector,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (include != null)
            {
                query = include(query);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query.SelectMany(selectMany, resultSelector).ToList();
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
