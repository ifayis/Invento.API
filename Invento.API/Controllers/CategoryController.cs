using Invento.Application.Common.Models;
using Invento.Application.Features.Categories.Command;
using Invento.Application.Features.Categories.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invento.API.Controllers
{
    [ApiController]
    [Route("api/category")]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _mediator.Send(new GetCategoriesQuery());

            var response = ApiResponse<object>.SuccessResponse(
                result,
                "Categories fetched successfully");

            return Ok(response);
        }


        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string q)
        {
            var result = await _mediator.Send(
                new SearchCategoryQuery
                {
                    Search = q
                });

            var response = ApiResponse<object>.SuccessResponse(
                result,
                "Categories search completed");

            return Ok(response);
        }


        [HttpPost("add")]
        public async Task<IActionResult> Create(
            [FromBody] CreateCategoryCommand cmd)
        {
            var result = await _mediator.Send(cmd);

            var response = ApiResponse<object>.SuccessResponse(
                result,
                "Category created successfully");

            return Ok(response);
        }


        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateCategoryCommand cmd)
        {
            cmd.Id = id;

            await _mediator.Send(cmd);

            var response = ApiResponse<object>.SuccessResponse(
                null,
                "Category updated successfully");

            return Ok(response);
        }


        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(
                new DeleteCategoryCommand
                {
                    Id = id
                });

            var response = ApiResponse<object>.SuccessResponse(
                null,
                "Category deleted successfully");

            return Ok(response);
        }
    }
}