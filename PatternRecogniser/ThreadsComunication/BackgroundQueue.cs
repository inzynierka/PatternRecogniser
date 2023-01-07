#region Copyright 2012-2014 by Roger Knapp, Licensed under the Apache License, Version 2.0
/* Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *   http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
#endregion
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
using CSharpTest.Net.Collections;

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

        public bool IsUsersModelInQueue(string login, string modelName = null );

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

        public bool IsUsersModelInQueue(string login, string modelName = null)
        {
            var qToList = _queue.ToList();
            if(modelName == null)
                return qToList.FindIndex(a => a.login == login ) >= 0;
            else
                return qToList.FindIndex(a => a.login == login && a.modelName == modelName) >= 0;
        }
    }

    public class BackgroundQueueLurchTable : IBackgroundTaskQueue
    {

        private readonly LurchTable<string, TrainingInfo> _queue = new LurchTable<string, TrainingInfo>(1000, LurchTableOrder.Insertion);


        public int Count => _queue.Count;

        public bool Remove(string login)
        {
            return _queue.TryRemove(login, null);
        }

        public TrainingInfo Dequeue(CancellationToken cancellationToken)
        {
            TrainingInfo item = _queue.Dequeue().Value;

            return item;
        }

        public void Enqueue(TrainingInfo item)
        {
            if (item == null) { throw new ArgumentNullException(nameof(item)); }
            _queue.TryAdd(item.login, item);
        }

        public int NumberInQueue(string login)
        {
            var qToList = _queue.ToList();
            int index = qToList.FindIndex(a => a.Key == login);
            if (index <0)
                return -1;
            return qToList.Count - 1 - index;
        }

        public bool IsUsersModelInQueue(string login, string modelName = null)
        {
            
            var userInQueue = _queue.TryGetValue(login, out var item);
            if (modelName == null)
                return userInQueue;
            else
                return userInQueue && item.modelName == modelName;
        }
    }

}
