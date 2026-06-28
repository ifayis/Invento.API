using Asp.Versioning;
using Invento.Application.Common;
using Invento.Application.Features.Profit.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invento.API.Controllers
{
    [Authorize(Policy = Permissions.Profit)]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]

    public class ProfitController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProfitController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet("net")]
        public async Task<IActionResult> NetProfit()
        {
            return Ok(await _mediator.Send(new GetNetProfitQuery()));
        }


        [HttpGet("range")]
        public async Task<IActionResult> ProfitByDate([FromQuery] GetProfitByDateRangeQuery query)
        {
            return Ok(await _mediator.Send(query));
        }


        [HttpGet("lastday")]
        public async Task<IActionResult> LastDay()
        {
            return Ok(await _mediator.Send(
                new GetProfitByDateRangeQuery
                {
                    FromDate = DateTime.UtcNow.Date,
                    ToDate = DateTime.UtcNow
                }
            ));
        }


        [HttpGet("lastweek")]
        public async Task<IActionResult> LastWeek()
        {
            return Ok(await _mediator.Send(
                new GetProfitByDateRangeQuery
                {
                    FromDate = DateTime.UtcNow.AddDays(-7),
                    ToDate =DateTime.UtcNow
                }
            ));
        }


        [HttpGet("lastmonth")]
        public async Task<IActionResult> LastMonth()
        {
            return Ok(await _mediator.Send(
                new GetProfitByDateRangeQuery
                {
                    FromDate = DateTime.UtcNow.AddMonths(-1),
                    ToDate = DateTime.UtcNow
                }
            ));
        }


        [HttpGet("product/{productId}")]
        public async Task<IActionResult> ProductProfit(Guid productId)
        {
            return Ok(await _mediator.Send(
                new GetProductProfitQuery
                {
                    ProductId = productId
                }
            ));
        }


        [HttpGet("dashboard")]
        public async Task<IActionResult> DashboardSummary()
        {
            return Ok(await _mediator.Send(new GetDashboardSummaryQuery()));
        }
    }
}