using Asp.Versioning;
using Invento.Application.Common;
using Invento.Application.Features.Users.Commands;
using Invento.Application.Features.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invento.API.Controllers
{
    [Authorize(Policy = Permissions.Users)]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]

    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(
            IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            CreateUserCommand command)
        {
            var result =
                await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] GetUsersQuery query)
        {
            var result =
                await _mediator.Send(query);

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(
            Guid id)
        {
            var result =
                await _mediator.Send(
                    new GetUserByIdQuery
                    {
                        Id = id
                    });

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id,
            UpdateUserCommand command)
        {
            command.Id = id;

            var result =
                await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(
            Guid id)
        {
            var result =
                await _mediator.Send(
                    new DeleteUserCommand
                    {
                        Id = id
                    });

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> SetStatus(
            Guid id,
            SetUserStatusCommand command)
        {
            command.Id = id;

            var result =
                await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPatch("{id:guid}/role")]
        public async Task<IActionResult> ChangeRole(
            Guid id,
            ChangeUserRoleCommand command)
        {
            command.Id = id;

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