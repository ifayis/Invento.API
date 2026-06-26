using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Security;
using Invento.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Users.Commands
{
    public class SetUserStatusCommandHandler
        : ICommandHandler<
            SetUserStatusCommand,
            ApiResponse<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public SetUserStatusCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<ApiResponse<string>> Handle(
            SetUserStatusCommand request,
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

            if (!UserAuthorizationService.CanActivateUser(
                    currentUser.Role))
            {
                return ApiResponse<string>
                    .FailureResponse(
                        new()
                        {
                            "You are not authorized to change user status."
                        });
            }

            if (currentUser.Id == targetUser.Id)
            {
                return ApiResponse<string>
                    .FailureResponse(
                        new()
                        {
                            "You cannot change your own account status."
                        });
            }

            if (targetUser.IsActive == request.IsActive)
            {
                return ApiResponse<string>
                    .FailureResponse(
                        new()
                        {
                            request.IsActive
                                ? "User is already active."
                                : "User is already inactive."
                        });
            }

            targetUser.IsActive = request.IsActive;

            if (!request.IsActive)
            {
                var refreshTokens =
                    await _context.RefreshTokens
                        .Where(x =>
                            x.UserId == targetUser.Id &&
                            !x.IsRevoked)
                        .ToListAsync(cancellationToken);

                foreach (var token in refreshTokens)
                {
                    token.IsRevoked = true;
                    token.RevokedAt = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync(
                cancellationToken);

            return ApiResponse<string>
                .SuccessResponse(
                    request.IsActive
                        ? "User activated successfully."
                        : "User deactivated successfully.");
        }
    }
}