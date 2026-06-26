using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Users.DTOs;
using Invento.Domain.Enums;

namespace Invento.Application.Features.Users.Commands
{
    public class CreateUserCommand
        : ICommand<ApiResponse<UserDto>>
    {
        public string FullName { get; set; }
            = string.Empty;

        public string Email { get; set; }
            = string.Empty;

        public string Password { get; set; }
            = string.Empty;

        public UserRole Role { get; set; }
    }
}