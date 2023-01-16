using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PatternRecogniser.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PatternRecogniser.Services
{
    public interface trainingInfoService
    {
        public  Task<List<TrainingInfo>> GetAsync();

        public  Task<TrainingInfo?> GetAsync(string id);

        public  Task CreateAsync(TrainingInfo trainingInfo);

        public  Task UpdateAsync(string id, TrainingInfo updateTrainingInfo);

        public  Task RemoveAsync(string id);
    }
    public class TrainingInfoMongoCollection
    {
        private readonly IMongoCollection<TrainingInfo> _trainingInfoCollection;

        public TrainingInfoMongoCollection(
            IOptions<TrainingInfoSettings> bookStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                bookStoreDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                bookStoreDatabaseSettings.Value.DatabaseName);

            _trainingInfoCollection = mongoDatabase.GetCollection<TrainingInfo>(
                bookStoreDatabaseSettings.Value.BooksCollectionName);
        }

        public async Task<List<TrainingInfo>> GetAsync() =>
            await _trainingInfoCollection.Find(_ => true).ToListAsync();

        public async Task<TrainingInfo?> GetAsync(string id) =>
            await _trainingInfoCollection.Find(x => x.login == id).FirstOrDefaultAsync();

        public async Task CreateAsync(TrainingInfo trainingInfo) =>
            await _trainingInfoCollection.InsertOneAsync(trainingInfo);

        public async Task UpdateAsync(string id, TrainingInfo updateTrainingInfo) =>
            await _trainingInfoCollection.ReplaceOneAsync(x => x.login == id, updateTrainingInfo);

        public async Task RemoveAsync(string id) =>
            await _trainingInfoCollection.DeleteOneAsync(x => x.login == id);
    }
}
