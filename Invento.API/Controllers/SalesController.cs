using Asp.Versioning;
using Invento.Application.Common;
using Invento.Application.Features.Sales.Command;
using Invento.Application.Features.Sales.Commands;
using Invento.Application.Features.Sales.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invento.API.Controllers
{
    [Authorize(Policy = Permissions.Sales)]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]

    public class SalesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SalesController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateSaleCommand command)
        {
            return Ok(await _mediator.Send(command));
        }


        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id,UpdateSaleCommand command)
        {
            command.Id = id;
            return Ok(await _mediator.Send(command));
        }


        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return Ok(await _mediator.Send(
                new DeleteSaleCommand
                {
                    Id = id
                }
            ));
        }


        [HttpPut("{id:guid}/restore")]
        public async Task<IActionResult> Restore(Guid id)
        {
            return Ok(await _mediator.Send(
                    new RestoreSaleCommand
                    {
                        Id = id
                    }
            ));
        }


        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(await _mediator.Send(
                new GetSaleByIdQuery
                {
                    Id = id
                }
            ));
        }


        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetSalesQuery query)
        {
            return Ok(await _mediator.Send(query));
        }


        [HttpGet("today")]
        public async Task<IActionResult> TodaySales()
        {
            return Ok(await _mediator.Send(
                new GetSalesQuery
                {
                    FromDate = DateTime.UtcNow.Date,
                    ToDate = DateTime.UtcNow.Date.AddDays(1).AddTicks(-1)
                }
            ));
        }


        [HttpGet("lastweek")]
        public async Task<IActionResult> LastWeekSales()
        {
            return Ok(await _mediator.Send(
                new GetSalesQuery
                {
                    FromDate = DateTime.UtcNow.AddDays(-7),
                    ToDate = DateTime.UtcNow
                }
            ));
        }


        [HttpGet("lastmonth")]
        public async Task<IActionResult> LastMonthSales()
        {
            return Ok(await _mediator.Send(
                new GetSalesQuery
                {
                    FromDate =DateTime.UtcNow.AddMonths(-1),
                    ToDate = DateTime.UtcNow
                }
            ));
        }


        [HttpGet("customer/{customerId:guid}")]
        public async Task<IActionResult> GetCustomerSales(Guid customerId)
        {
            return Ok(await _mediator.Send(
                new GetCustomerSalesQuery
                {
                    CustomerId = customerId
                }
            ));
        }
    }
}