using System.Security.Cryptography;
using System.Text;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Auth.Commands
{
    public class ResetPasswordCommandHandler
        : ICommandHandler<
            ResetPasswordCommand,
            ApiResponse<string>>
    {
        private readonly IApplicationDbContext _context;

        public ResetPasswordCommandHandler(
            IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<string>> Handle(
            ResetPasswordCommand request,
            CancellationToken cancellationToken)
        {
            var tokenHash =
                Convert.ToHexString(
                    SHA256.HashData(
                        Encoding.UTF8.GetBytes(
                            request.Token)));

            var resetToken =
                await _context.PasswordResetTokens
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(
                        x =>
                            x.TokenHash == tokenHash &&
                            !x.IsUsed,
                        cancellationToken);

            if (resetToken is null)
            {
                return ApiResponse<string>
                    .FailureResponse(
                        new List<string>
                        {
                            "Invalid password reset token."
                        },
                        "Password reset failed");
            }

            if (resetToken.ExpiresAt < DateTime.UtcNow)
            {
                return ApiResponse<string>
                    .FailureResponse(
                        new List<string>
                        {
                            "Password reset token has expired."
                        },
                        "Password reset failed");
            }

            resetToken.User.PasswordHash =
                PasswordHasher.Hash(
                    request.NewPassword);

            resetToken.IsUsed = true;
            resetToken.UsedAt = DateTime.UtcNow;

            var refreshTokens =
                await _context.RefreshTokens
                    .Where(x =>
                        x.UserId == resetToken.UserId &&
                        !x.IsRevoked)
                    .ToListAsync(cancellationToken);

            foreach (var token in refreshTokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(
                cancellationToken);

            return ApiResponse<string>
                .SuccessResponse(
                    "Password reset successfully. Please login again.");
        }
    }
}