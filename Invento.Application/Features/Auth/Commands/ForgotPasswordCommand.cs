using Invento.Application.Abstractions;
using Invento.Application.Common;

namespace Invento.Application.Features.Auth.Commands
{
    public class ForgotPasswordCommand
        : ICommand<ApiResponse<string>>
    {
        public string Email { get; set; } = string.Empty;
    }
}