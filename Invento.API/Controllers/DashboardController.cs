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
    }

}
