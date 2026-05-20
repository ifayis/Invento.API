using Invento.Application.Abstractions;
using Invento.Application.Features.Auth.DTOs;
using Invento.Application.Common;

namespace Invento.Application.Features.Auth.Commands
{
    public class LoginCommand
        : ICommand<ApiResponse<AuthResponseDto>>
    {
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
