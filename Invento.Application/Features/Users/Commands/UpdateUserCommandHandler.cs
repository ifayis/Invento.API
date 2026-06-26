using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Security;
using Invento.Application.Features.Users.DTOs;
using Invento.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Users.Commands
{
    public class UpdateUserCommandHandler
        : ICommandHandler<
            UpdateUserCommand,
            ApiResponse<UserDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public UpdateUserCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<ApiResponse<UserDto>> Handle(
            UpdateUserCommand request,
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

            var currentUser =
                await _context.Users
                    .FirstOrDefaultAsync(
                        x => x.Id == currentUserId,
                        cancellationToken);

            if (currentUser is null)
            {
                return ApiResponse<UserDto>
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
                return ApiResponse<UserDto>
                    .FailureResponse(
                        new()
                        {
                            "User not found."
                        });
            }

            if (!UserAuthorizationService.CanManageUser(
                    currentUser.Role,
                    currentUser.TenantId,
                    targetUser.TenantId,
                    targetUser.Role))
            {
                return ApiResponse<UserDto>
                    .FailureResponse(
                        new()
                        {
                            "You are not authorized to update this user."
                        });
            }

            var emailExists =
                await _context.Users
                    .AnyAsync(
                        x =>
                            x.Id != targetUser.Id &&
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

            targetUser.FullName =
                request.FullName.Trim();

            targetUser.Email =
                request.Email.Trim().ToLower();

            await _context.SaveChangesAsync(
                cancellationToken);

            return ApiResponse<UserDto>
                .SuccessResponse(
                    new UserDto
                    {
                        Id = targetUser.Id,
                        TenantId = targetUser.TenantId,
                        FullName = targetUser.FullName,
                        Email = targetUser.Email,
                        Role = targetUser.Role,
                        IsActive = targetUser.IsActive,
                        MustChangePassword =
                            targetUser.MustChangePassword,
                        CreatedAt = targetUser.CreatedAt,
                        CreatedBy = targetUser.CreatedBy,
                        CreatedByUserId =
                            targetUser.CreatedByUserId
                    },
                    "User updated successfully.");
        }
    }
}