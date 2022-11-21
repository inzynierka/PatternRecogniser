using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.IO.Compression;
using Microsoft.AspNetCore.Http;
using PatternRecogniser.Models;
//using CSharpTest.Net.Collections;

namespace PatternRecogniser.ThreadsComunication
{

    public class TrainingInfo
    {
        public TrainingInfo(int userId, IFormFile trainingSet, string modelName, DistributionType distributionType)
        {
            this.userId = userId;
            this.trainingSet = trainingSet;
            this.modelName = modelName;
            this.distributionType = distributionType;

        }

        public int userId;
        public IFormFile trainingSet;
        public string modelName;
        public DistributionType distributionType;
    }


    public interface IBackgroundTaskQueue
    {
        int Count { get; }
        public void Enqueue(TrainingInfo item);

        public TrainingInfo Dequeue(
            CancellationToken cancellationToken);

        public int NumberInQueue(int userId);

        public bool Remove(int userId);
    }

    public class BackgroundQueueBlockingCollection : IBackgroundTaskQueue
    {

        private readonly BlockingCollection<TrainingInfo> _queue = new BlockingCollection<TrainingInfo>(new ConcurrentQueue<TrainingInfo>());

        public int Count => _queue.Count;


        public TrainingInfo Dequeue(
            CancellationToken cancellationToken)
        {
            TrainingInfo item = _queue.Take();
            return item;
        }

        public void Enqueue(TrainingInfo item)
        {
            if (item == null) { throw new ArgumentNullException(nameof(item)); }
            _queue.Add(item);
        }

        public int NumberInQueue(int userId)
        {
            var qToList = _queue.ToList();
            return qToList.FindIndex(a => a.userId == userId);
        }

        public bool Remove(int userId)
        {
            throw new NotImplementedException();
        }
    }

}
