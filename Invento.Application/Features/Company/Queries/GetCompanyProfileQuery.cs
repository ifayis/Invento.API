using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Company.DTOs;

namespace Invento.Application.Features.Company.Queries
{
    public class GetCompanyProfileQuery
        : IQuery<ApiResponse<CompanyProfileDto>>
    {
    }
}
