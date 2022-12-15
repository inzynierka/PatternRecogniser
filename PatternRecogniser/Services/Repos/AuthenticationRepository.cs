using Microsoft.EntityFrameworkCore;
using PatternRecogniser.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PatternRecogniser.Services.NewFolder
{
    public interface IAuthenticationRepo
    {
        public List<User> GetUsers(Expression<Func<User, bool>> filter = null);
        public  Task AddUserToDB(User user);
        public Task SaveChangesAsync();
    }

    public class AuthenticationRepository : IAuthenticationRepo
    {

        private PatternRecogniserDBContext _context;
        private DbSet<User> _dbSet;

        public AuthenticationRepository(PatternRecogniserDBContext context)
        {
            _context = context;
            _dbSet = context.Set<User>();
        }

        public async Task AddUserToDB(User user)
        {
            _context.user.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }


        public List<User> GetUsers(Expression<Func<User, bool>> filter = null)
        {
            IQueryable<User> query = _dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return query.ToList();
        }
    }
}
