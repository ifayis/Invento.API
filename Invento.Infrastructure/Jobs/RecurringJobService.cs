using Invento.Application.Common.Jobs;

namespace Invento.Infrastructure.Jobs
{
    public class RecurringJobService
        : IRecurringJobService
    {
        public Task ExecuteLowStockCheck()
        {
            return Task.CompletedTask;
        }

        public Task ExecuteSalesTargetCheck()
        {
            return Task.CompletedTask;
        }

        public Task ExecuteProfitTargetCheck()
        {
            return Task.CompletedTask;
        }

        public Task ExecuteReceivableCheck()
        {
            return Task.CompletedTask;
        }

        public Task ExecutePayableCheck()
        {
            return Task.CompletedTask;
        }
    }
}