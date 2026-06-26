using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Security;
using Invento.Application.Features.Users.DTOs;
using Invento.Application.Interfaces;
using Invento.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Users.Commands
{
    public class ChangeUserRoleCommandHandler
        : ICommandHandler<
            ChangeUserRoleCommand,
            ApiResponse<UserDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public ChangeUserRoleCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<ApiResponse<UserDto>> Handle(
            ChangeUserRoleCommand request,
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

            if (!UserAuthorizationService.CanChangeRole(
                    currentUser.Role))
            {
                return ApiResponse<UserDto>
                    .FailureResponse(
                        new()
                        {
                            "You are not authorized to change user roles."
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

            if (currentUser.TenantId != targetUser.TenantId)
            {
                return ApiResponse<UserDto>
                    .FailureResponse(
                        new()
                        {
                            "User not found."
                        });
            }

            if (currentUser.Id == targetUser.Id)
            {
                return ApiResponse<UserDto>
                    .FailureResponse(
                        new()
                        {
                            "You cannot change your own role."
                        });
            }

            if (targetUser.Role == request.Role)
            {
                return ApiResponse<UserDto>
                    .FailureResponse(
                        new()
                        {
                            "User already has this role."
                        });
            }

            targetUser.Role = request.Role;

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
                    "User role updated successfully.");
        }
    }
}