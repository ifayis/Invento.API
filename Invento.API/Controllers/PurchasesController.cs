using Invento.Application.Features.Purchases.Commands;
using Invento.Application.Features.Purchases.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invento.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PurchasesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PurchasesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            CreatePurchaseCommand command)
        {
            return Ok(
                await _mediator.Send(command));
        }

        [HttpPut]
        public async Task<IActionResult> Update(
            UpdatePurchaseCommand command)
        {
            return Ok(
                await _mediator.Send(command));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return Ok(
                await _mediator.Send(
                    new DeletePurchaseCommand
                    {
                        Id = id
                    }));
        }

        [HttpPut("{id}/restore")]
        public async Task<IActionResult> Restore(Guid id)
        {
            return Ok(
                await _mediator.Send(
                    new RestorePurchaseCommand
                    {
                        Id = id
                    }));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] GetPurchasesQuery query)
        {
            return Ok(
                await _mediator.Send(query));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(
                await _mediator.Send(
                    new GetPurchaseByIdQuery
                    {
                        Id = id
                    }));
        }
    }
}