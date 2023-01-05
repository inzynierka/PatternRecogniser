using System;
using System.Threading;
using System.Threading.Tasks;

namespace PatternRecogniser.Services
{
    public class TimeOutClass
    {
        private int _waitingTimeInSeconds;
        private Thread _worker;
        private string _timeoutMessage = "";
        private CancellationToken _stoppingToken;
        private CancellationTokenSource _timeoutCancellationTokenSource;
        public TimeOutClass(Action workingFunction, int waitingTimeInSeconds, string timeoutMessage, CancellationToken cancellationToken)
        {
            _worker = new Thread(new ThreadStart(workingFunction));
            _waitingTimeInSeconds = waitingTimeInSeconds;
            _timeoutMessage = timeoutMessage;
            _timeoutCancellationTokenSource = new CancellationTokenSource();
            _stoppingToken = cancellationToken;
        }

        private void waiter()
        {
            _worker.Start();
            _worker.Join();
            SendEndOfWorkSignal();
        }

        private void SendEndOfWorkSignal()
        {
            _timeoutCancellationTokenSource.Cancel();
        }


        public void StartWork()
        {
            Task wait = new Task(waiter);
            wait.Start();
            if(!_stoppingToken.WaitHandle.WaitOne(TimeSpan.FromSeconds(_waitingTimeInSeconds)))
            {
                try
                {
                    _worker.Abort();
                }
                catch(Exception e)
                {
                    
                }
                throw new TimeoutException(_timeoutMessage);
            }
        }

        
    }
}
