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
using System.IO;
//using CSharpTest.Net.Collections;

namespace PatternRecogniser.ThreadsComunication
{

    public class TrainingInfo
    {
        public TrainingInfo(string login, IFormFile trainingSet, string modelName, DistributionType distributionType, int trainingPercent, int sets)
        {
            this.login = login;
            this.trainingSet = ReadFully(trainingSet.OpenReadStream());
            this.modelName = modelName;
            this.distributionType = distributionType;
            this.trainingPercent = trainingPercent;
            this.sets = sets;
        }

        public string login;
        public byte[] trainingSet;
        public string modelName;
        public DistributionType distributionType;
        public int trainingPercent;
        public int sets;

        

        private  byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

       

    }


    public interface IBackgroundTaskQueue
    {
        int Count { get; }
        public void Enqueue(TrainingInfo item);

        public TrainingInfo Dequeue(
            CancellationToken cancellationToken);

        public int NumberInQueue(string login);

        public bool Remove(string login);

        public bool IsUserModelInQueue(string login, string modelName);

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

        public int NumberInQueue(string login)
        {
            var qToList = _queue.ToList();
            return qToList.FindIndex(a => a.login == login);
        }

        public bool Remove(string login)
        {
            throw new NotImplementedException();
        }

        public bool IsUserModelInQueue(string login, string modelName)
        {
            var qToList = _queue.ToList();
            return qToList.FindIndex(a => a.login == login && a.modelName == modelName) >= 0;
        }
    }

}
