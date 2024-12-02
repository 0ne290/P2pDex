using Core.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[Route("api")]
public class ApiController : Controller
{
    public ApiController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [Route("create-sell-order")]
    [HttpPost]
    public async Task<IActionResult> CreateSellOrder([FromBody] CreateSellOrderCommand request)
    {
        var response = await _mediator.Send(request);
        
        return Ok();
    }

    private readonly IMediator _mediator;
}