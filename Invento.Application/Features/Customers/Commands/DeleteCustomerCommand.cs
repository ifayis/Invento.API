using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Customer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Application.Features.Customer.Commands
{
    public class DeleteCustomerCommand
        : ICommand<ApiResponse<CustomerDto>>
    {
        public Guid Id { get; set; }
    }
}
