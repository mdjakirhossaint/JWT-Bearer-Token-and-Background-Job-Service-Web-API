namespace WebAPIJwtTokenAuthentication.BackgroundService
{
    public abstract class ScopedProcessor : BackgroundService
    {
        private IServiceScopeFactory _scopeFactory;
        public ScopedProcessor(IServiceScopeFactory scopeFactory) //: base()
        {
            _scopeFactory= scopeFactory;
        }
        protected override async Task Process()
        {
            using (var scope=_scopeFactory.CreateScope())
            {
                await ProcessInScope(scope.ServiceProvider);
            }
        }
        public abstract Task ProcessInScope(IServiceProvider serviceProvider);
    }
}
