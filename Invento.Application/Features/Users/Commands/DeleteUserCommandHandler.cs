using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Security;
using Invento.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Users.Commands
{
    public class DeleteUserCommandHandler
        : ICommandHandler<
            DeleteUserCommand,
            ApiResponse<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public DeleteUserCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<ApiResponse<string>> Handle(
            DeleteUserCommand request,
            CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(
                _currentUser.UserId,
                out var currentUserId))
            {
                return ApiResponse<string>
                    .FailureResponse(
                        new()
                        {
                            "Unauthorized."
                        });
            }

            var currentUser =
                await _context.Users
                    .FirstOrDefaultAsync(
                        x => x.Id == currentUserId,
                        cancellationToken);

            if (currentUser is null)
            {
                return ApiResponse<string>
                    .FailureResponse(
                        new()
                        {
                            "Unauthorized."
                        });
            }

            var targetUser =
                await _context.Users
                    .FirstOrDefaultAsync(
                        x => x.Id == request.Id,
                        cancellationToken);

            if (targetUser is null)
            {
                return ApiResponse<string>
                    .FailureResponse(
                        new()
                        {
                            "User not found."
                        });
            }

            if (!UserAuthorizationService.CanDeleteUser(
                    currentUser.Role))
            {
                return ApiResponse<string>
                    .FailureResponse(
                        new()
                        {
                            "You are not authorized to delete users."
                        });
            }

            if (currentUser.Id == targetUser.Id)
            {
                return ApiResponse<string>
                    .FailureResponse(
                        new()
                        {
                            "You cannot delete your own account."
                        });
            }

            targetUser.IsDeleted = true;
            targetUser.DeletedAt = DateTime.UtcNow;
            targetUser.DeletedBy = currentUser.Id.ToString();

            await _context.SaveChangesAsync(
                cancellationToken);

            return ApiResponse<string>
                .SuccessResponse(
                    "User deleted successfully.");
        }
    }
}