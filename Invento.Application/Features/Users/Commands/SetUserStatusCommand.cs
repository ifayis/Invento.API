using Invento.Application.Abstractions;
using Invento.Application.Common;

namespace Invento.Application.Features.Users.Commands
{
    public class SetUserStatusCommand
        : ICommand<ApiResponse<string>>
    {
        public Guid Id { get; set; }

        public bool IsActive { get; set; }
    }
}