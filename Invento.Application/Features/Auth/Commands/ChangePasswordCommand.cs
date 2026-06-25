using Invento.Application.Abstractions;
using Invento.Application.Common;

namespace Invento.Application.Features.Auth.Commands
{
    public class ChangePasswordCommand
        : ICommand<ApiResponse<string>>
    {
        public string CurrentPassword { get; set; }
            = string.Empty;

        public string NewPassword { get; set; }
            = string.Empty;

        public string ConfirmPassword { get; set; }
            = string.Empty;
    }
}