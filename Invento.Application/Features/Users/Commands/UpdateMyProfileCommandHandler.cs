using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Users.DTOs;
using Invento.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Users.Commands
{
    public class UpdateMyProfileCommandHandler
        : ICommandHandler<
            UpdateMyProfileCommand,
            ApiResponse<UserDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public UpdateMyProfileCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<ApiResponse<UserDto>> Handle(
            UpdateMyProfileCommand request,
            CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(
                _currentUser.UserId,
                out var currentUserId))
            {
                return ApiResponse<UserDto>
                    .FailureResponse(
                        new()
                        {
                            "Unauthorized."
                        });
            }

            var user =
                await _context.Users
                    .FirstOrDefaultAsync(
                        x => x.Id == currentUserId,
                        cancellationToken);

            if (user is null)
            {
                return ApiResponse<UserDto>
                    .FailureResponse(
                        new()
                        {
                            "User not found."
                        });
            }

            var emailExists =
                await _context.Users
                    .AnyAsync(
                        x =>
                            x.Id != user.Id &&
                            x.Email == request.Email &&
                            !x.IsDeleted,
                        cancellationToken);

            if (emailExists)
            {
                return ApiResponse<UserDto>
                    .FailureResponse(
                        new()
                        {
                            "Email already exists."
                        });
            }

            user.FullName =
                request.FullName.Trim();

            user.Email =
                request.Email.Trim().ToLower();

            await _context.SaveChangesAsync(
                cancellationToken);

            return ApiResponse<UserDto>
                .SuccessResponse(
                    new UserDto
                    {
                        Id = user.Id,
                        TenantId = user.TenantId,
                        FullName = user.FullName,
                        Email = user.Email,
                        Role = user.Role,
                        IsActive = user.IsActive,
                        MustChangePassword =
                            user.MustChangePassword,
                        CreatedAt = user.CreatedAt,
                        CreatedBy = user.CreatedBy,
                        CreatedByUserId =
                            user.CreatedByUserId
                    },
                    "Profile updated successfully.");
        }
    }
}