namespace WebAPIJwtTokenAuthentication.BackgroundService
{
    public abstract class BackgroundService : IHostedService
    {
        private Task _executingTask;
        private readonly CancellationTokenSource _tokenSource= new CancellationTokenSource();
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _executingTask = ExecuteAsync(_tokenSource.Token);
            if (_executingTask.IsCompleted)
            { 
                return _executingTask;
            }
            return Task.CompletedTask;
        }
        public virtual  async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_executingTask==null)
            {
                return ;
            }
            try
            {
                _tokenSource.Cancel();
            }
            finally
            {
                await Task.WhenAny(_executingTask,Task.Delay(Timeout.Infinite,cancellationToken));
            }
        }

        protected virtual async Task ExecuteAsync(CancellationToken tokenSource)
        {
            do
            {
                await Process();
                await Task.Delay(5000, tokenSource);
            }
            while (!tokenSource.IsCancellationRequested);

        }
        protected abstract Task Process();

    }
}
