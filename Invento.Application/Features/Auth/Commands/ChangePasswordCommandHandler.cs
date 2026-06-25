using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Auth.Commands
{
    public class ChangePasswordCommandHandler
        : ICommandHandler<
            ChangePasswordCommand,
            ApiResponse<string>>
    {
        private readonly IApplicationDbContext _context;

        private readonly ICurrentUserService _currentUser;

        public ChangePasswordCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<ApiResponse<string>> Handle(
            ChangePasswordCommand request,
            CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(
                _currentUser.UserId,
                out var userId))
            {
                return ApiResponse<string>
                    .FailureResponse(
                        new List<string>
                        {
                            "User not found."
                        });
            }

            var user =
                await _context.Users
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id == userId &&
                            !x.IsDeleted,
                        cancellationToken);

            if (user is null)
            {
                return ApiResponse<string>
                    .FailureResponse(
                        new List<string>
                        {
                            "User not found."
                        });
            }

            if (!PasswordHasher.Verify(
                request.CurrentPassword,
                user.PasswordHash))
            {
                return ApiResponse<string>
                    .FailureResponse(
                        new List<string>
                        {
                            "Current password is incorrect."
                        });
            }

            if (PasswordHasher.Verify(
                request.NewPassword,
                user.PasswordHash))
            {
                return ApiResponse<string>
                    .FailureResponse(
                        new List<string>
                        {
                            "New password must be different from the current password."
                        });
            }

            user.PasswordHash =
                PasswordHasher.Hash(
                    request.NewPassword);

            user.MustChangePassword = false;

            var refreshTokens =
                await _context.RefreshTokens
                    .Where(x =>
                        x.UserId == user.Id &&
                        !x.IsRevoked)
                    .ToListAsync(
                        cancellationToken);

            foreach (var token in refreshTokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(
                cancellationToken);

            return ApiResponse<string>
                .SuccessResponse(
                    "Password changed successfully. Please login again.");
        }
    }
}