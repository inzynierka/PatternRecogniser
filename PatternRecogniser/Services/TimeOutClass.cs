using System;
using System.Threading;
using System.Threading.Tasks;

namespace PatternRecogniser.Services
{
    public class TimeoutClass
    {
        private int _waitingTimeInSeconds;
        private Task _worker;
        private string _timeoutMessage = "";
        private readonly CancellationTokenSource _timeoutCancellationTokenSource;
        private readonly CancellationToken _stoppingToken;
        private readonly CancellationTokenSource _combaineTokensSource;
        public CancellationToken cancellationToken => _combaineTokensSource.Token;
        public TimeoutClass(int waitingTimeInSeconds, string timeoutMessage, CancellationToken stoppingToken)
        {
            _waitingTimeInSeconds = waitingTimeInSeconds;
            _timeoutMessage = timeoutMessage;
            _timeoutCancellationTokenSource = new CancellationTokenSource();
            _stoppingToken = stoppingToken;
            _combaineTokensSource = CancellationTokenSource.CreateLinkedTokenSource(_stoppingToken, _timeoutCancellationTokenSource.Token);
        }

        // czeka na zakończenie się funkcji lub na sygnał
        private void Waiter()
        {
            _worker.Start();
            _worker.Wait(cancellationToken: _combaineTokensSource.Token);
            SendEndOfWorkSignal();
        }

        private void SendEndOfWorkSignal()
        {
            _timeoutCancellationTokenSource.Cancel();
        }

        
        private bool WasEndOfWorkSignalRecivedInGivenTime()
        {
            return cancellationToken.WaitHandle.WaitOne(TimeSpan.FromSeconds(_waitingTimeInSeconds));
        }

        // rozpoczyna funkcje jak i odliczanie
        public void StartWork(Action workingFunction)
        {
            _worker = new Task(workingFunction, _combaineTokensSource.Token);
            Task wait = new Task(Waiter, _combaineTokensSource.Token);
            wait.Start();
            if( ! WasEndOfWorkSignalRecivedInGivenTime())
            {
                SendEndOfWorkSignal();
                throw new TimeoutException(_timeoutMessage);
            }
        }

        
    }
}
