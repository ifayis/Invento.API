using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Auth.DTOs;

namespace Invento.Application.Features.Auth.Commands
{
    public class RefreshTokenCommand
        : ICommand<ApiResponse<AuthResponseDto>>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}