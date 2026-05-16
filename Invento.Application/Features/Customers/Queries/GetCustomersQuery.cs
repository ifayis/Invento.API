using Invento.Application.Abstractions;
using Invento.Application.Features.Customer.DTOs;
using Invento.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Application.Features.Customers.Queries
{
    public class GetCustomersQuery
        : IQuery<
            ApiResponse<PagedResponse<CustomerDto>>>
    {
        public string? Search { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
