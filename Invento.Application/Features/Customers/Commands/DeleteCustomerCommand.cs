using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invento.Application.Common;
using Invento.Application.Abstractions;

namespace Invento.Application.Features.Customer.Commands
{
    public class DeleteCustomerCommand
        : ICommand<ApiResponse<Guid>>
    {
        public Guid Id { get; set; }
    }
}
