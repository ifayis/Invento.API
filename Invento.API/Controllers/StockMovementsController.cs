using Invento.Application.Features.StockMovements.Commands;
using Invento.Application.Features.StockMovements.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invento.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StockMovementsController
    : ControllerBase
{
    private readonly IMediator _mediator;

    public StockMovementsController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateStockMovementCommand command)
    {
        return Ok(await _mediator.Send(command));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery]
        GetStockMovementsQuery query)
    {
        return Ok(await _mediator.Send(query));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        return Ok(await _mediator.Send(
            new GetStockMovementByIdQuery
            {
                Id = id
            }));
    }
}