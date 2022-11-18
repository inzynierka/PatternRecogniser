using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace PatternRecogniser.Services
{

    public class TrainingInfo
    {
        public TrainingInfo(int userId, byte[] trainingSet)
        {
            this.userId = userId;
            this.trainingSet = trainingSet;
        }
        public int userId;
        public byte[] trainingSet;
    }


    public interface IBackgroundTaskQueue
    {
        int Count { get; }
        public ValueTask AddAsync(TrainingInfo item);

        public ValueTask<TrainingInfo> DequeueAsync(
            CancellationToken cancellationToken);

        public int PlaceInQueue(int userId);
    }


    // nie da się zaimplementować sprawdzenia który jest w kolejce bez kombinowania, najdokładniejsze zdejmowanie i wkładanie
    public class BackgroundQueueChannel : IBackgroundTaskQueue
    {
        private readonly Channel<TrainingInfo> _queue;
        private int _count = 0;

        public int Count => _count; 

        public BackgroundQueueChannel(int capacity)
        {
            var options = new BoundedChannelOptions(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait
                
            };
            _queue = Channel.CreateBounded<TrainingInfo>(options);
        }


        public async ValueTask<TrainingInfo> DequeueAsync(
            CancellationToken cancellationToken)
        {
            var workItem = await _queue.Reader.ReadAsync(cancellationToken);
            Interlocked.Decrement(ref _count);
            return workItem;
        }

        public async ValueTask AddAsync(TrainingInfo item)
        {
            if (item == null) { throw new ArgumentNullException(nameof(item)); }
            await _queue.Writer.WriteAsync(item);
            Interlocked.Increment(ref _count);
        }

        public int PlaceInQueue(int userId)
        {
            throw new NotImplementedException();
        }
    }

    // busy waiting przy zdejmowaniu z kolejki ( potencjalnie można to rozwiązać poprzez zmergowanie z Channel,
    // możemy dopisywać wartości do ConcurrentQueue i chanel i jednocześnie robić na nim operacje
    //
    // trzeba sprytnie usuwać z kolejki ( można by zapisywać w bazie kiedy użytkownik wysłał ostatnie 
    // rządanie trenowania i odrzucać te które nie są równe
    public class BackgroundQueueConcurrentQueue : IBackgroundTaskQueue
    {
        
        private readonly ConcurrentQueue<TrainingInfo> _queue = new ConcurrentQueue<TrainingInfo>();

        public int Count => _queue.Count;

        public async ValueTask<TrainingInfo> DequeueAsync(
            CancellationToken cancellationToken)
        {
            TrainingInfo item;
            while(!_queue.TryDequeue(out item))
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            return item;
        }

        public async ValueTask AddAsync(TrainingInfo item)
        {
            if (item == null) { throw new ArgumentNullException(nameof(item)); }
            _queue.Enqueue(item);
        }

        public int PlaceInQueue(int userId)
        {
            var qToList = _queue.ToList();
            qToList.Reverse();
            return qToList.Count - qToList.FindIndex(a => a.userId == userId);
        }
    }


    // ogromny spadek cpu albo nie ogarniam bo strona się ładuje i ładuje
    public class BackgroundQueueBlockingCollection : IBackgroundTaskQueue
    {

        private readonly BlockingCollection<TrainingInfo> _queue = new BlockingCollection<TrainingInfo>(1000);

        public int Count => _queue.Count;

        public async ValueTask<TrainingInfo> DequeueAsync(
            CancellationToken cancellationToken)
        {
            TrainingInfo item = _queue.Take();
            return item;
        }

        public async ValueTask AddAsync(TrainingInfo item)
        {
            if (item == null) { throw new ArgumentNullException(nameof(item)); }
            _queue.Add(item);
        }

        public int PlaceInQueue(int userId)
        {
            var qToList = _queue.ToList();
            qToList.Reverse();
            return qToList.Count - qToList.FindIndex(a => a.userId == userId);
        }
    }

}
