using Asp.Versioning;
using Invento.Application.Features.Users.Commands;
using Invento.Application.Features.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invento.API.Controllers
{
    [ApiController]
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]

    public class ProfileController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProfileController(
            IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var result =
                await _mediator.Send(
                    new GetMyProfileQuery());

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile(
            UpdateMyProfileCommand command)
        {
            var result =
                await _mediator.Send(command);

            return result.Success
                ? Ok(result)
                : BadRequest(result);
        }
    }
}