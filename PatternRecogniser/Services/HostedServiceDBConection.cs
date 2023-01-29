using Microsoft.Extensions.DependencyInjection;
using PatternRecogniser.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PatternRecogniser.Services
{
    public interface IHostedServiceDBConection
    {
        public User GetUser(string login);
        public Task SaveModel(ExtendedModel extendedModel);
    }

    public class HostedServiceDBConection: IHostedServiceDBConection
    {
        private IServiceScopeFactory _serviceScopeFactory;

        public HostedServiceDBConection(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public User GetUser(string login)
        {
            User user;
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<PatternRecogniserDBContext>();
                user = dbContext.user.First(u => u.login == login);
            }
            return user;
        }

        public async Task SaveModel(ExtendedModel extendedModel)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<PatternRecogniserDBContext>();

                dbContext.Add(extendedModel);


                await dbContext.SaveChangesAsync();
            }
        }

        
    }
}
