using MediatR;
using Microsoft.AspNetCore.Mvc;
using SPS.Application.Customers.Commands;

namespace SPS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;
    public CustomersController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create(CreateCustomerCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(id);
    }
}