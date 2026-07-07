namespace Invento.Application.Common.Jobs
{
    public interface IRecurringJobService
    {
        Task ExecuteLowStockCheck();

        Task ExecuteSalesTargetCheck();

        Task ExecuteProfitTargetCheck();

        Task ExecuteReceivableCheck();

        Task ExecutePayableCheck();

        Task ExecuteRefreshTokenCleanup();
    }
}