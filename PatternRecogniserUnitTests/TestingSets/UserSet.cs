using PatternRecogniser.Models;
using PatternRecogniser.Services.NewFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PatternRecogniserUnitTests.TestingSets
{
    public class UserTestSet : IAuthenticationRepo
    {
        public List<User> Users { get; set; }
        public UserTestSet(List<User> users)
        {
            Users = users;
        }

        public List<User> GetUsers(Expression<Func<User, bool>> filter = null)
        {
            if (filter == null)
                return Users;
            IQueryable<User> queryable = Users.AsQueryable<User>();
            return queryable.Where(filter).ToList();
        }

        public Task AddUserToDB(User user)
        {
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
