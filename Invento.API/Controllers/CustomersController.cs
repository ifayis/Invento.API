using Asp.Versioning;
using Invento.Application.Common;
using Invento.Application.Features.Customer.Commands;
using Invento.Application.Features.Customers.Commands;
using Invento.Application.Features.Customers.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invento.API.Controllers
{
    [Authorize(Policy = Permissions.Customers)]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CustomersController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateCustomerCommand command)
        {
            return Ok(await _mediator.Send(command));
        }


        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id,UpdateCustomerCommand command)
        {
            command.Id = id;
            return Ok(await _mediator.Send(command));
        }


        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return Ok(await _mediator.Send(
                new DeleteCustomerCommand
                {
                    Id = id
                }
            ));
        }


        [HttpPut("{id:guid}/restore")]
        public async Task<IActionResult> Restore(Guid id)
        {
            return Ok(await _mediator.Send(
                    new RestoreCustomerCommand
                    {
                        Id = id
                    }
            ));
        }


        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetCustomersQuery query)
        {
            return Ok(await _mediator.Send(query));
        }


        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(await _mediator.Send(
                new GetCustomerByIdQuery
                {
                    Id = id
                }
            ));
        }


        [HttpGet("top-customers")]
        public async Task<IActionResult> TopCustomers(
                [FromQuery] int top = 10)
        {
            return Ok(
                await _mediator.Send(
                    new GetTopCustomersQuery
                    {
                        Top = top
                    }
                )
            );
        }


        [HttpGet("{customerId:guid}/ledger")]
        public async Task<IActionResult> Ledger(
            Guid customerId)
        {
            return Ok(
                await _mediator.Send(
                    new GetCustomerLedgerQuery
                    {
                        CustomerId = customerId
                    }));
        }


        [HttpGet("{customerId:guid}/sales-summary")]
        public async Task<IActionResult> SalesSummary(
            Guid customerId)
        {
            return Ok(
                await _mediator.Send(
                    new GetCustomerSalesSummaryQuery
                    {
                        CustomerId = customerId
                    }
                )
            );
        }


        [HttpGet("inactive")]
        public async Task<IActionResult> InactiveCustomers(
        [FromQuery] int days = 30)
        {
            return Ok(
                await _mediator.Send(
                    new GetInactiveCustomersQuery
                    {
                        Days = days
                    }
                )
            );
        }


        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            return Ok(
                await _mediator.Send(
                    new GetCustomerDashboardQuery()
                )
            );
        }
    }
}