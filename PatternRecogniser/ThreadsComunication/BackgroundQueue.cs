﻿#region Copyright 2012-2014 by Roger Knapp, Licensed under the Apache License, Version 2.0
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


    //#region Copyright 2012-2014 by Roger Knapp, Licensed under the Apache License, Version 2.0
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
    public class BackgroundQueueLurchTable : IBackgroundTaskQueue
    {
        private class Value
        {
            public TrainingInfo info;
            public int numberInQueue;
            public Value(TrainingInfo trainingInfo, int numberInQueue)
            {
                info = trainingInfo;
                this.numberInQueue = numberInQueue;
            }
        }
        private readonly LurchTable<string, Value> _queue = new LurchTable<string, Value>(1000, LurchTableOrder.Insertion);


        public int Count => _queue.Count;

        public BackgroundQueueLurchTable()
        {
            _queue.ItemRemoved += DecrementNumbersInQueue;
        }

        private void DecrementNumbersInQueue(KeyValuePair< string, Value> keyValuePair)
        {
            var qToList = _queue.ToList();
            foreach(var item in qToList)
            {
                if(item.Value.numberInQueue > keyValuePair.Value.numberInQueue)
                    _queue.TryUpdate(item.Key, new Value(item.Value.info, item.Value.numberInQueue - 1));
            }
        }

        public bool Remove(string login)
        {
            return _queue.TryRemove(login, null);
        }

        public TrainingInfo Dequeue(CancellationToken cancellationToken)
        {
            TrainingInfo item = _queue.Dequeue().Value.info;

            return item;
        }

        public void Enqueue(TrainingInfo item)
        {
            if (item == null) { throw new ArgumentNullException(nameof(item)); }
            _queue.TryAdd(item.login, new Value(item, Count));
        }

        public int NumberInQueue(string login)
        {
            Value v;
            var contains = _queue.TryGetValue(login, out v);
            if (!contains)
                return -1;
            
             return v.numberInQueue;
        }

        public bool IsUsersModelInQueue(string login, string modelName = null)
        {
            
            var userInQueue = _queue.TryGetValue(login, out var item);
            if (modelName == null)
                return userInQueue;
            else
                return userInQueue && item.info.modelName == modelName;
        }
    }

}
