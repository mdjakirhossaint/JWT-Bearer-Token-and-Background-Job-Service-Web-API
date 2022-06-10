using NCrontab;

namespace WebAPIJwtTokenAuthentication.BackgroundService
{
    public abstract class ScheduleProcessor : ScopedProcessor
    {
        private CrontabSchedule _schedule;
        private DateTime _nextRun;

        protected abstract string Schedule { get; }
       // private readonly IServiceScopeFactory scopeFactory;

        public ScheduleProcessor(IServiceScopeFactory scopeFactory) : base(scopeFactory)
        {
            _schedule = CrontabSchedule.Parse(Schedule);
            _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
        }
        public override Task ProcessInScope(IServiceProvider serviceProvider)
        { 
            throw new NotImplementedException();
        }
        protected virtual async Task ExecuteAsync(CancellationToken tokenSource)
        {
            do
            {
                var now =DateTime.Now;
                if (now>_nextRun)
                {
                    await Process();
                    _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
                }
                await Task.Delay(1,tokenSource);//5 secound delay the task
            }
            while (!tokenSource.IsCancellationRequested);

        }
    }
}
