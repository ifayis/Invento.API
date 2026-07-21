using Asp.Versioning;
using Invento.Application.Common;
using Invento.Application.Features.Receivables.Commands;
using Invento.Application.Features.Receivables.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invento.API.Controllers
{
    [Authorize(Policy = Permissions.Receivables)]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]

    public class ReceivablesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReceivablesController(
            IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("payment")]
        public async Task<IActionResult> RecordPayment(
            RecordCustomerPaymentCommand command)
        {
            return Ok(
                await _mediator.Send(command));
        }

        [HttpGet("outstanding")]
        public async Task<IActionResult> Outstanding()
        {
            return Ok(
                await _mediator.Send(
                    new GetCustomerOutstandingQuery()));
        }

        [HttpGet("customer/{customerId:guid}/history")]
        public async Task<IActionResult> PaymentHistory(
            Guid customerId)
        {
            return Ok(
                await _mediator.Send(
                    new GetCustomerPaymentHistoryQuery
                    {
                        CustomerId = customerId
                    }));
        }
    }
}