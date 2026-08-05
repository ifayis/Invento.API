using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Users.DTOs;
using Invento.Domain.Enums;
using System.Text.Json.Serialization;

namespace Invento.Application.Features.Users.Commands
{
    public class ChangeUserRoleCommand
        : ICommand<ApiResponse<UserDto>>
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        public UserRole Role { get; set; }
    }
}