using Invento.Application.Features.Targets.Commands;
using Invento.Application.Features.Targets.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invento.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TargetsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TargetsController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet("low-stock")]
        public async Task<IActionResult> LowStock()
        {
            return Ok(await _mediator.Send(new GetLowStockProductsQuery()));
        }


        [HttpGet("critical-stock")]
        public async Task<IActionResult> CriticalStock()
        {
            return Ok(await _mediator.Send(new GetCriticalStockProductsQuery()));
        }


        [HttpGet("monthly")]
        public async Task<IActionResult> GetTargets()
        {
            return Ok(await _mediator.Send(new GetTenantTargetsQuery()));
        }


        [HttpPut("monthly")]
        public async Task<IActionResult> UpdateTargets(UpdateTenantTargetsCommand command)
        {
            return Ok(await _mediator.Send(command));
        }


        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            return Ok(
                await _mediator.Send(
                    new GetTargetDashboardQuery()
                )
            );
        }


        [HttpGet("reorder-products")]
        public async Task<IActionResult> ReorderProducts()
        {
            return Ok(
                await _mediator.Send(
                    new GetReorderProductsQuery()
                )
            );
        }
    }
}