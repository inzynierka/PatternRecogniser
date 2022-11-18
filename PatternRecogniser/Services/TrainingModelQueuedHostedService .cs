using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PatternRecogniser.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace PatternRecogniser.Services
{
    public class TrainingModelQueuedHostedService : BackgroundService
    {
        private const int _maxTasks = 3; 
        private int _taskCount = 0;
        private readonly ILogger<TrainingModelQueuedHostedService> _logger;
        private IBackgroundTaskQueue _backgroundJobs;
        private IServiceScopeFactory _serviceScopeFactory;

        public TrainingModelQueuedHostedService(
            ILogger<TrainingModelQueuedHostedService> logger,
            IBackgroundTaskQueue backgroundJobs,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _backgroundJobs = backgroundJobs;
            _serviceScopeFactory = serviceScopeFactory;
        }

        

        protected override   Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation( $"Queued Hosted Service is running.{Environment.NewLine}");

            BackgroundProcessing(stoppingToken);
            return Task.CompletedTask;
        }

        private async void BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var info =
                    await _backgroundJobs.DequeueAsync(stoppingToken);

                
                    await Train(info.userId, info.trainingSet, stoppingToken, _serviceScopeFactory);
                
                
            }
        }

        private async Task Train(int userId, byte[] trainingSet, CancellationToken stoppingToken, IServiceScopeFactory serviceScopeFactory)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<PatternRecogniserDBContext>();
                _logger.LogInformation($"request of user {dbContext.user.First(a => a.userId == userId).login} is processing");
            }
            await Task.Delay(TimeSpan.FromSeconds(100), stoppingToken);
        }
    }
}
