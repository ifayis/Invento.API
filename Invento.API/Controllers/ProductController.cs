using Invento.Application.Features.Products.Commands;
using Invento.Application.Features.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invento.API.Controllers
{
    [ApiController]
    [Route("api/products")]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _mediator.Send(new GetProductsQuery()));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
            => Ok(await _mediator.Send(new GetProductByIdQuery { Id = id }));

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string q)
            => Ok(await _mediator.Send(new SearchProductsQuery { Search = q }));

        [HttpPost("add")]
        public async Task<IActionResult> Create(CreateProductCommand cmd)
            => Ok(await _mediator.Send(cmd));

        [HttpPatch("update/{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateProductCommand cmd)
        {
            cmd.Id = id;
            await _mediator.Send(cmd);
            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteProductCommand { Id = id });
            return NoContent();
        }
    }
}
