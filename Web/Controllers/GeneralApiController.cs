using Core.Application.General.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[Route("api")]
[Authorize]
public class GeneralApiController : Controller
{
    public GeneralApiController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [Route("create-trader")]
    [HttpPost]
    public async Task<IActionResult> CreateTrader([FromBody] CreateTraderCommand? request)
    {
        if (request == null)
            return BadRequest(Web.Response.Fail(new { message = "Request format is invalid." }).ToJson());
        
        var result = await _mediator.Send(request);

        return ActionResultHelper.CreateResponse(result, HttpContext);
    }

    [Route("calculate-final-crypto-amount-for-transfer")]
    [HttpGet]
    public async Task<IActionResult> CalculateFinalCryptoAmountForTransfer(
        [FromBody] CalculateFinalCryptoAmountForTransferCommand? request)
    {
        if (request == null)
            return BadRequest(Web.Response.Fail(new { message = "Request format is invalid." }).ToJson());
        
        var result = await _mediator.Send(request);

        return ActionResultHelper.CreateResponse(result, HttpContext);
    }
    
    [Route("get-exchanger-account-address")]
    [HttpGet]
    public async Task<IActionResult> GetExchangerAccountAddress()
    {
        var result = await _mediator.Send(new GetExchangerAccountAddressCommand());

        return ActionResultHelper.CreateResponse(result, HttpContext);
    }

    private readonly IMediator _mediator;
}
