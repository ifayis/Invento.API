using Asp.Versioning;
using Invento.Application.Common;
using Invento.Application.Features.Balance.Commands;
using Invento.Application.Features.Balance.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invento.API.Controllers
{
    [Authorize(Policy = Permissions.Balance)]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]

    public class BalanceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BalanceController(
            IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            return Ok(
                await _mediator.Send(
                    new GetBalanceDashboardQuery()));
        }

        [HttpGet("transactions")]
        public async Task<IActionResult> Transactions(
            [FromQuery] GetCashTransactionsQuery query)
        {
            return Ok(
                await _mediator.Send(query));
        }

        [HttpGet("transactions/{id:guid}")]
        public async Task<IActionResult> TransactionById(
            Guid id)
        {
            return Ok(
                await _mediator.Send(
                    new GetCashTransactionByIdQuery
                    {
                        Id = id
                    }));
        }

        [HttpPost("income")]
        public async Task<IActionResult> AddIncome(
            CreateManualIncomeCommand command)
        {
            return Ok(
                await _mediator.Send(command));
        }

        [HttpPost("expense")]
        public async Task<IActionResult> AddExpense(
            CreateManualExpenseCommand command)
        {
            return Ok(
                await _mediator.Send(command));
        }
    }
}