using Asp.Versioning;
using Invento.Application.Common;
using Invento.Application.Features.Company.Commands;
using Invento.Application.Features.Company.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invento.API.Controllers
{
    [Authorize(Policy = Permissions.Company)]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]

    public class CompanyController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CompanyController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet]
        public async Task<IActionResult>  GetProfile()
        {
            return Ok(await _mediator.Send(
                new GetCompanyProfileQuery()));
        }


        [HttpPut]
        public async Task<IActionResult> Update(UpdateCompanyProfileCommand command)
        {
            return Ok(await _mediator.Send(command));
        }
    }
}