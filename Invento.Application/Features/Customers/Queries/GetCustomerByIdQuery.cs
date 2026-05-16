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
    public class GetCustomerByIdQuery
        : IQuery<ApiResponse<CustomerDto>>
    {
        public Guid Id { get; set; }
    }
}
