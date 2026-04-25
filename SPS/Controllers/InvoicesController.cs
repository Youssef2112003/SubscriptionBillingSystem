using MediatR;
using Microsoft.AspNetCore.Mvc;
using SPS.Application.Invoices.Commands;
using SPS.Application.Invoices.Queries;

namespace SPS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoicesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public InvoicesController(IMediator mediator) => _mediator = mediator;

        [HttpPost("{id}/pay")]
        public async Task<IActionResult> Pay(Guid id)
        {
            await _mediator.Send(new PayInvoiceCommand(id));
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetInvoicesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
