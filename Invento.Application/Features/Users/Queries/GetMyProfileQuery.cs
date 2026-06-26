using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Users.DTOs;

namespace Invento.Application.Features.Users.Queries
{
    public class GetMyProfileQuery
        : IQuery<ApiResponse<UserDto>>
    {
    }
}