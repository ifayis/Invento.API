using Invento.Application.Features.Dashboard.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invento.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DashboardController(
        IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> Summary()
        {
            return Ok(
                await _mediator.Send(
                    new GetDashboardSummaryQuery()));
        }

        [HttpGet("recent-sales")]
        public async Task<IActionResult> RecentSales(
            [FromQuery] int count = 10)
        {
            return Ok(
                await _mediator.Send(
                    new GetRecentSalesQuery
                    {
                        Count = count
                    }));
        }

        [HttpGet("recent-purchases")]
        public async Task<IActionResult> RecentPurchases(
            [FromQuery] int count = 10)
        {
            return Ok(
                await _mediator.Send(
                    new GetRecentPurchasesQuery
                    {
                        Count = count
                    }));
        }

        [HttpGet("top-products")]
        public async Task<IActionResult> TopProducts(
    [FromQuery] int count = 10)
        {
            return Ok(
                await _mediator.Send(
                    new GetTopProductsQuery
                    {
                        Count = count
                    }));
        }

        [HttpGet("top-customers")]
        public async Task<IActionResult> TopCustomers(
            [FromQuery] int count = 10)
        {
            return Ok(
                await _mediator.Send(
                    new GetTopCustomersQuery
                    {
                        Count = count
                    }));
        }

        [HttpGet("top-suppliers")]
        public async Task<IActionResult> TopSuppliers(
            [FromQuery] int count = 10)
        {
            return Ok(
                await _mediator.Send(
                    new GetTopSuppliersQuery
                    {
                        Count = count
                    }));
        }

        [HttpGet("monthly-sales-chart")]
        public async Task<IActionResult> MonthlySalesChart()
        {
            return Ok(
                await _mediator.Send(
                    new GetMonthlySalesChartQuery()));
        }

        [HttpGet("monthly-purchases-chart")]
        public async Task<IActionResult> MonthlyPurchasesChart()
        {
            return Ok(
                await _mediator.Send(
                    new GetMonthlyPurchasesChartQuery()));
        }

        [HttpGet("monthly-profit-chart")]
        public async Task<IActionResult> MonthlyProfitChart()
        {
            return Ok(
                await _mediator.Send(
                    new GetMonthlyProfitChartQuery()));
        }
    }

}
