using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Users.DTOs;
using Invento.Domain.Enums;

namespace Invento.Application.Features.Users.Commands
{
    public class ChangeUserRoleCommand
        : ICommand<ApiResponse<UserDto>>
    {
        public Guid Id { get; set; }

        public UserRole Role { get; set; }
    }
}