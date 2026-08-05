using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Users.DTOs;
using System.Text.Json.Serialization;

namespace Invento.Application.Features.Users.Commands
{
    public class UpdateUserCommand
        : ICommand<ApiResponse<UserDto>>
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
    }
}