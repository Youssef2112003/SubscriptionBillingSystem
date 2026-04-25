using MediatR;
using Microsoft.AspNetCore.Mvc;
using SPS.Application.Subscriptions.Commands;

namespace SPS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public SubscriptionsController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> Create(CreateSubscriptionCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(id);
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            await _mediator.Send(new CancelSubscriptionCommand(id));
            return NoContent();
        }
    }
}
