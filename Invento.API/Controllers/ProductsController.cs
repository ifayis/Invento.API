using Invento.Application.Features.Products.Commands;
using Invento.Application.Features.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invento.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(
            new GetProductByIdQuery
            {
                Id = id
            });

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
    [FromQuery] GetProductsQuery query)
    {
        var result = await _mediator.Send(query);

        return Ok(result);
    }


    [HttpPost]
    public async Task<IActionResult> Create(
        CreateProductCommand command)
    {
        var result = await _mediator.Send(command);

        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update(
        UpdateProductCommand command)
    {
        var result = await _mediator.Send(command);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(
            new DeleteProductCommand
            {
                Id = id
            });

        return Ok(result);
    }

}