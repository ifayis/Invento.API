using Asp.Versioning;
using Invento.Application.Common;
using Invento.Application.Features.Products.Commands;
using Invento.Application.Features.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invento.API.Controllers
{
    [Authorize(Policy = Permissions.Products)]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]

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
                }
            );

            return Ok(result);
        }


        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetProductsQuery query)
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
        public async Task<IActionResult> Update(UpdateProductCommand command)
        {
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpPost("{productId:guid}/images")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImage(
            Guid productId,
            IFormFile image)
        {
            var command =
                new UploadProductImageCommand
                {
                    ProductId = productId,
                    Image = image
                };

            var result =
                await _mediator.Send(command);

            return Ok(result);
        }


        [HttpGet("{productId:guid}/images")]
        public async Task<IActionResult> GetImages(
            Guid productId)
        {
            return Ok(
                await _mediator.Send(
                    new GetProductImagesQuery
                    {
                        ProductId = productId
                    }));
        }


        [HttpDelete("images/{imageId:guid}")]
        public async Task<IActionResult> DeleteImage(
            Guid imageId)
        {
            return Ok(
                await _mediator.Send(
                    new DeleteProductImageCommand
                    {
                        ImageId = imageId
                    }));
        }

        [HttpPut("images/{imageId:guid}/primary")]
        public async Task<IActionResult> SetPrimary(
            Guid imageId)
        {
            return Ok(
                await _mediator.Send(
                    new SetPrimaryProductImageCommand
                    {
                        ImageId = imageId
                    }));
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(
                new DeleteProductCommand
                {
                    Id = id
                }
            );

            return Ok(result);
        }


        [HttpPut("{id}/restore")]
        public async Task<IActionResult> Unhide(Guid id)
        {
            var result = await _mediator.Send(
                new RestoreProductCommand
                {
                    Id = id
                }
            );

            return Ok(result);
        }
    }
}