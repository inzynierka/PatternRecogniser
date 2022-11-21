using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PatternRecogniser.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using PatternRecogniser.ThreadsComunication;

namespace PatternRecogniser.Services
{
    public class TrainingModelQueuedHostedService : BackgroundService
    {
        // potencjalnie możemy pracować na raz na x wątkach by trenować. Wtedy będzie potrzebny
        // słownik w implementacji trainingUpdate
        //private const int _maxTasks = 3; 
        //private int _taskCount = 0;
        private readonly ILogger<TrainingModelQueuedHostedService> _logger;
        private IBackgroundTaskQueue _trainInfoQueue;
        private IServiceScopeFactory _serviceScopeFactory;
        private ITrainingUpdate _trainingUpdate;

        public TrainingModelQueuedHostedService(
            ILogger<TrainingModelQueuedHostedService> logger,
            IBackgroundTaskQueue backgroundJobs,
            IServiceScopeFactory serviceScopeFactory,
            ITrainingUpdate trainingUpdate)
        {
            _logger = logger;
            _trainInfoQueue = backgroundJobs;
            _serviceScopeFactory = serviceScopeFactory;
            _trainingUpdate = trainingUpdate;
        }

        

        protected override   Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation( $"Queued Hosted Service is running.{Environment.NewLine}");

            BackgroundProcessing(stoppingToken);
            return Task.CompletedTask;
        }

        private async void BackgroundProcessing(CancellationToken stoppingToken)
        {
            // z jakiegoś powodu bez tego nie działa blockingCollection
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                var info =
                     _trainInfoQueue.Dequeue(stoppingToken);

               
                
                    await Train(info, stoppingToken);
                
                
            }
        }

        private async Task Train(TrainingInfo info,  CancellationToken stoppingToken)
        {

            _trainingUpdate.SetNewUserModel(info.userId, info.modelName);

            ExtendedModel extendedModel = new ExtendedModel();
            //extendedModel.TrainModel();
            for(int i = 0; i < 5; i++)
            {
                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                _trainingUpdate.Update($"info dla usera {info.userId}: {DateTime.Now}\n"); // zapisuje info
            }
            // tutaj byśmy zapisywali wyniki trenowania
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<PatternRecogniserDBContext>();
                dbContext.extendedModel.Add(new ExtendedModel()
                {
                    name = info.modelName,
                    userId = info.userId,

                });
                _logger.LogInformation($"request of user {dbContext.user.First(a => a.userId == info.userId).login} is processing {info.modelName}\n");
            }
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}
