using Invento.Application.Common.Models;
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
        public async Task<IActionResult> Get([FromQuery] string? q)
        {
            var result = await _mediator.Send(
                new GetProductsQuery
                {
                    Search = q
                });

            var response = ApiResponse<object>.SuccessResponse(
                result,
                "Products fetched successfully");

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(
                new GetProductByIdQuery
                {
                    Id = id
                });

            var response = ApiResponse<object>.SuccessResponse(
                result,
                "Product fetched successfully");

            return Ok(response);
        }


        [HttpPost("add")]
        public async Task<IActionResult> Create(
            [FromBody] CreateProductCommand cmd)
        {
            var result = await _mediator.Send(cmd);

            var response = ApiResponse<object>.SuccessResponse(
                result,
                "Product created successfully");

            return Ok(response);
        }


        [HttpPatch("update/{id}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateProductCommand cmd)
        {
            cmd.Id = id;

            await _mediator.Send(cmd);

            var response = ApiResponse<object>.SuccessResponse(
                null,
                "Product updated successfully");

            return Ok(response);
        }


        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(
                new DeleteProductCommand
                {
                    Id = id
                });

            var response = ApiResponse<object>.SuccessResponse(
                null,
                "Product deleted successfully");

            return Ok(response);
        }
    }
}