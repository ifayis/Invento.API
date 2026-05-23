using Invento.Application.Abstractions;
using Invento.Application.Common;

namespace Invento.Application.Features.Auth.Commands
{
    public class LogoutCommand
        : ICommand<ApiResponse<string>>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}