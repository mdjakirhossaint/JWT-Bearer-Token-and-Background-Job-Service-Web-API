using WebAPIJwtTokenAuthentication.BackgroundService.Services;
using WebAPIJwtTokenAuthentication.Data;
using WebAPIJwtTokenAuthentication.Entities;

namespace WebAPIJwtTokenAuthentication.BackgroundService.ScheduleTask
{
    public class TaskOne : ScheduleProcessor
    {
        protected override string Schedule => "*/1 * * * *";
        public TaskOne(IServiceScopeFactory scopeFactory) : base(scopeFactory)
        {
        }

        public override Task ProcessInScope(IServiceProvider serviceProvider)
        {
            Console.WriteLine("First Task Print ; " + DateTime.Now.ToString());

            IProductRepository productRepository= serviceProvider.GetRequiredService<IProductRepository>();
            productRepository.Save();

            return Task.CompletedTask;
        }
    }
}
