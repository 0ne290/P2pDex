using Core.Application.Commands;
using Core.Application.Errors;
using FluentResults;
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
        var response = (Result)await _mediator.Send(request);

        if (response.IsSuccess)
            return Ok();
        if (response.HasError<DevelopmentError>())
            return ServerError(response.Errors.Select(e => e.Message));
        return BadRequest(response.Errors.Select(e => e.Message));
    }

    private readonly IMediator _mediator;
}