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
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.IO.Compression;
using PatternRecogniser.Messages.HostedService;

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
        private int _timeoutInSeconds = 60*30;
        private HostedServiceStringMessages _messages = new HostedServiceStringMessages();

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
            _trainingUpdate.SetNewUserModel(info.login, info.modelName);
            User user;
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<PatternRecogniserDBContext>();
                user = dbContext.user.First(u => u.login == info.login);
            }


            
            var model = new ExtendedModel()
            {
                name = info.modelName,
                userLogin = info.login,
                distribution = info.distributionType
            };

            //Stream stream = info.trainingSet.OpenReadStream ();
            //info.
            // coś nam to nie działa 
            try
            {

                ; // parametry na razie ustawione

                var timeout = new TimeOutClass(
                    () => model.TrainModel(info.distributionType, _trainingUpdate, info.trainingSet, info.trainingPercent, info.sets),
                    _timeoutInSeconds,
                    _messages.timeout);

                timeout.StartWork();

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetService<PatternRecogniserDBContext>();

                    dbContext.Add(model);


                    await dbContext.SaveChangesAsync();

                    _logger.LogInformation($"request of user {dbContext.user.First(a => a.login == info.login).login} is processing {info.modelName}\n");
                }
            }
            catch(Exception e)
            {
                _trainingUpdate.Update(e.Message);
                _logger.LogInformation($"error in hosted service: {e.Message}\n");

            }



            _trainingUpdate.SetNewUserModel(null, null);
        }
    }
}
