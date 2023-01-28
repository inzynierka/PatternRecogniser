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
using PatternRecogniser.Services;

namespace PatternRecogniser.ThreadsComunication
{


    public interface IBackgroundTaskQueue
    {
        int Count { get; }
        public Task Enqueue(TrainingInfo item);

        public Task<TrainingInfo> Dequeue(
            CancellationToken cancellationToken);

        public int NumberInQueue(string login);

        public bool Remove(string login);

        public bool IsUsersModelInQueue(string login, string modelName = null );

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


        private readonly ItrainingInfoService infoService;
        private readonly LurchTable<string, Value> _queue = new LurchTable<string, Value>(1000, LurchTableOrder.Insertion);

        public int Count => _queue.Count;
       

        private class InfoContainer
        {
            public string id { get; set; }
            public string login { get; set; }
            public string modelName { get; set; }
            public InfoContainer(TrainingInfo trainingInfo)
            {
                id = trainingInfo.id;
                login = trainingInfo.login;
                modelName = trainingInfo.modelName;
            }
        }




        public BackgroundQueueLurchTable(ItrainingInfoService trainingInfoService)
        {
            this.infoService = trainingInfoService;
            var lastData = infoService.GetAsync().Result;
            lastData.Sort((a, b) => a.addedTime > b.addedTime ? 1: -1);
            foreach(var item in lastData)
            {
                _queue.TryAdd(item.login, new Value(item, Count));
            }
            init();
        }

        private void init()
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
            Value va;
            bool deleted = _queue.TryRemove(login, out va);
            if (deleted)
                infoService.RemoveAsync(va.info.id).Wait();
            return deleted;
        }

        public async Task<TrainingInfo> Dequeue(CancellationToken cancellationToken)
        {
            var item = _queue.Dequeue().Value.info;
            var data = await infoService.GetThenDelateAsync(item.id);
            return data;
        }

        public async Task Enqueue(TrainingInfo item)
        {
            if (item == null) { throw new ArgumentNullException(nameof(item)); }

            await infoService.CreateAsync(item);
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

        private class Value
        {
            public InfoContainer info;
            public int numberInQueue;
            public Value(TrainingInfo trainingInfo, int numberInQueue)
            {
                info = new InfoContainer(trainingInfo);
                this.numberInQueue = numberInQueue;
            }
            public Value(InfoContainer trainingInfo, int numberInQueue)
            {
                info = trainingInfo;
                this.numberInQueue = numberInQueue;
            }
        }
    }

}
