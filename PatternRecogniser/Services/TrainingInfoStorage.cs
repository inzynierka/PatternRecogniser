﻿using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PatternRecogniser.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PatternRecogniser.Services
{
    public interface ItrainingInfoService
    {
        public  Task<List<TrainingInfo>> GetAsync();

        public  Task<TrainingInfo?> GetAsync(string id);
        public Task<TrainingInfo?> GetThenDelateAsync(string id)
        {
            var info = GetAsync(id).Result;
            RemoveAsync(id);
            return Task.FromResult(info);
        }

        public Task CreateAsync(TrainingInfo trainingInfo);

        public  Task UpdateAsync(string id, TrainingInfo updateTrainingInfo);

        public  Task RemoveAsync(string id);
    }
    public class TrainingInfoMongoCollection: ItrainingInfoService
    {
        private readonly IMongoCollection<TrainingInfo> _trainingInfoCollection;

        public TrainingInfoMongoCollection(
            IOptions<TrainingInfoSettings> trainingInfoDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                trainingInfoDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                trainingInfoDatabaseSettings.Value.DatabaseName);

            _trainingInfoCollection = mongoDatabase.GetCollection<TrainingInfo>(
                trainingInfoDatabaseSettings.Value.CollectionName);
        }

        public async Task<List<TrainingInfo>> GetAsync() =>
            await _trainingInfoCollection.Find(_ => true).ToListAsync();

        public async Task<TrainingInfo?> GetAsync(string id) =>
            await _trainingInfoCollection.Find(x => x.id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(TrainingInfo trainingInfo) =>
            await _trainingInfoCollection.InsertOneAsync(trainingInfo);

        public async Task UpdateAsync(string id, TrainingInfo updateTrainingInfo) =>
            await _trainingInfoCollection.ReplaceOneAsync(x => x.id == id, updateTrainingInfo);

        public async Task RemoveAsync(string id) =>
            await _trainingInfoCollection.DeleteOneAsync(x => x.id == id);

        }
    }
