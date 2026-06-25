using Invento.Application.Common;
using Invento.Application.Features.Suppliers.Commands;
using Invento.Application.Features.Suppliers.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invento.API.Controllers
{
    [Authorize(Policy = Permissions.Suppliers)]
    [ApiController]
    [Route("api/[controller]")]

    public class SuppliersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SuppliersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            CreateSupplierCommand command)
        {
            return Ok(
                await _mediator.Send(command));
        }

        [HttpPut]
        public async Task<IActionResult> Update(
            UpdateSupplierCommand command)
        {
            return Ok(
                await _mediator.Send(command));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(
            Guid id)
        {
            return Ok(
                await _mediator.Send(
                    new DeleteSupplierCommand
                    {
                        Id = id
                    }));
        }

        [HttpPut("{id}/restore")]
        public async Task<IActionResult> Restore(
            Guid id)
        {
            return Ok(
                await _mediator.Send(
                    new RestoreSupplierCommand
                    {
                        Id = id
                    }));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] GetSuppliersQuery query)
        {
            return Ok(
                await _mediator.Send(query));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(
            Guid id)
        {
            return Ok(
                await _mediator.Send(
                    new GetSupplierByIdQuery
                    {
                        Id = id
                    }));
        }
    }
}