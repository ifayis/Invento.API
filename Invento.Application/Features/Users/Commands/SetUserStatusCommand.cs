using Invento.Application.Abstractions;
using Invento.Application.Common;
using System.Text.Json.Serialization;

namespace Invento.Application.Features.Users.Commands
{
    public class SetUserStatusCommand
        : ICommand<ApiResponse<string>>
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        public bool IsActive { get; set; }
    }
}