using Asp.Versioning;
using Invento.Application.Common;
using Invento.Application.Features.StockMovements.Commands;
using Invento.Application.Features.StockMovements.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invento.API.Controllers
{
    [Authorize(Policy = Permissions.StockMovements)]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]

    public class StockMovementsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StockMovementsController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateStockMovementCommand command)
        {
            return Ok(await _mediator.Send(command));
        }


        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetStockMovementsQuery query)
        {
            return Ok(await _mediator.Send(query));
        }


        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(await _mediator.Send(
                new GetStockMovementByIdQuery
                {
                    Id = id
                }
            ));
        }


        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            return Ok(
                await _mediator.Send(
                    new GetInventoryDashboardQuery()
                )
            );
        }


        [HttpGet("fast-moving")]
        public async Task<IActionResult> FastMoving(
        [FromQuery] int top = 10)
        {
            return Ok(
                await _mediator.Send(
                    new GetFastMovingProductsQuery
                    {
                        Top = top
                    }
                )
            );
        }


        [HttpGet("dead-stock")]
        public async Task<IActionResult> DeadStock(
        [FromQuery] int days = 90)
        {
            return Ok(
                await _mediator.Send(
                    new GetDeadStockProductsQuery
                    {
                        Days = days
                    }
                )
            );
        }
    }
}