using Invento.Application.Abstractions;
using Invento.Application.Common;

namespace Invento.Application.Features.Auth.Commands
{
    public class ResetPasswordCommand
        : ICommand<ApiResponse<string>>
    {
        public string Token { get; set; } = string.Empty;

        public string NewPassword { get; set; } = string.Empty;

        public string ConfirmPassword { get; set; } = string.Empty;
    }
}