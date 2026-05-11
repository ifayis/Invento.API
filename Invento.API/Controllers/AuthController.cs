using Invento.Application.Common.Models;
using Invento.Application.Features.Auth.Commands;
using Invento.Application.Features.Auth.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Invento.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost("signup")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterCommand command)
        {
            var result = await _mediator.Send(command);

            var response = ApiResponse<AuthResponse>.SuccessResponse(
                result,
                "Registration successful");

            return Ok(response);
        }


        [HttpPost("signin")]
        public async Task<IActionResult> Login(
            [FromBody] LoginCommand command)
        {
            var result = await _mediator.Send(command);

            var response = ApiResponse<AuthResponse>.SuccessResponse(
                result,
                "Login successful");

            return Ok(response);
        }


        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(
            [FromBody] RefreshTokenCommand command)
        {
            var result = await _mediator.Send(command);

            var response = ApiResponse<AuthResponse>.SuccessResponse(
                result,
                "Token refreshed successfully");

            return Ok(response);
        }
    }
}