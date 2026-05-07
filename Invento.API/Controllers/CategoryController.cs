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
            => Ok(await _mediator.Send(new GetCategoriesQuery()));

        [HttpGet("search")]
        public async Task<IActionResult> Search(string q)
            => Ok(await _mediator.Send(new SearchCategoryQuery { Search = q }));

        [HttpPost("add")]
        public async Task<IActionResult> Create(CreateCategoryCommand cmd)
            => Ok(await _mediator.Send(cmd));

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateCategoryCommand cmd)
        {
            cmd.Id = id;
            await _mediator.Send(cmd);
            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteCategoryCommand { Id = id });
            return NoContent();
        }
    }
}
