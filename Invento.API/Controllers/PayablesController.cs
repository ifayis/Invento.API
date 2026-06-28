using Asp.Versioning;
using Invento.Application.Common;
using Invento.Application.Features.Payables.Commands;
using Invento.Application.Features.Payables.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invento.API.Controllers
{
    [Authorize(Policy = Permissions.Payables)]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]

    public class PayablesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PayablesController(
            IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("payment")]
        public async Task<IActionResult> RecordPayment(
            RecordSupplierPaymentCommand command)
        {
            return Ok(
                await _mediator.Send(command));
        }

        [HttpGet("outstanding")]
        public async Task<IActionResult> Outstanding()
        {
            return Ok(
                await _mediator.Send(
                    new GetSupplierOutstandingQuery()));
        }

        [HttpGet("supplier/{supplierId}/history")]
        public async Task<IActionResult> PaymentHistory(
            Guid supplierId)
        {
            return Ok(
                await _mediator.Send(
                    new GetSupplierPaymentHistoryQuery
                    {
                        SupplierId = supplierId
                    }));
        }
    }
}