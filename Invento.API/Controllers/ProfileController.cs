using Invento.Application.Features.Users.Commands;
using Invento.Application.Features.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invento.API.Controllers
{
    [ApiController]
    [Route("api/profile")]
    [Authorize]
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

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}