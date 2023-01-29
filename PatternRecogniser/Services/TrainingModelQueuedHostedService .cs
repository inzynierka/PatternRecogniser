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
        private readonly ILogger<TrainingModelQueuedHostedService> _logger;
        private IBackgroundTaskQueue _trainInfoQueue;
        private IHostedServiceDBConection _hostedServiceDBConection;
        private ITrainingUpdate _trainingUpdate;
        private int _timeoutInSeconds;
        private HostedServiceStringMessages _messages = new HostedServiceStringMessages();

        public TrainingModelQueuedHostedService(
            ILogger<TrainingModelQueuedHostedService> logger,
            IBackgroundTaskQueue backgroundJobs,
            IHostedServiceDBConection hostedServiceDBConection,
            ITrainingUpdate trainingUpdate,
            TrainingSettings trainingSettings)
        {
            _logger = logger;
            _trainInfoQueue = backgroundJobs;
            _hostedServiceDBConection = hostedServiceDBConection;
            _trainingUpdate = trainingUpdate;
            _timeoutInSeconds = trainingSettings.TimeoutInSeconds;
        }

        

        protected override   Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation( $"Queued Hosted Service is running.{Environment.NewLine}");

            BackgroundProcessing(stoppingToken);
            return Task.CompletedTask;
        }

        private async void BackgroundProcessing(CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                var info =
                     _trainInfoQueue.Dequeue(stoppingToken).Result;


                await Train(info, stoppingToken);

            }
        }

        private async Task Train(TrainingInfo info,  CancellationToken stoppingToken)
        {
            try
            {
                _trainingUpdate.SetNewUserModel(info.login, info.modelName);
                User user = _hostedServiceDBConection.GetUser(info.login);



                var model = new ExtendedModel()
                {
                    name = info.modelName,
                    userLogin = info.login,
                    distribution = info.distributionType
                };



                var timeout = new TimeoutClass(
                    _timeoutInSeconds,
                    _messages.timeout,
                    stoppingToken);

                // potrzebuje przegazać cancellationToken stworzony w timeoutclass
                timeout.StartWork(() => model.TrainModel(info.distributionType, _trainingUpdate, info.trainingSet, info.trainingPercent, info.sets, timeout.cancellationToken));

                await _hostedServiceDBConection.SaveModel(model);

                _logger.LogInformation($"request of user  {info.login} is processing {info.modelName}\n");
                _trainingUpdate.SetNewUserModel(null, null);

            }
            catch (Exception e)
            {
                _trainingUpdate.Update(e.Message);
                _logger.LogInformation($"error in hosted service: {e.Message}\n");

            }



        }
    }
}
