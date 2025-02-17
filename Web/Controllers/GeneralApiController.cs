using Core.Application.Api.General.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[Route("api")]
public class GeneralApiController : Controller
{
    public GeneralApiController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [Route("ensure-existed-of-trader/{userId:long}:{userName}")]
    [HttpGet]
    public async Task<IActionResult> EnsureExistedOfTrader(long userId, string userName)
    {
        var result = await _mediator.Send(new EnsureExistedOfTraderCommand
            { Id = userId, Name = userName == "null" ? null : userName });

        return Web.Response.Create200(result);
    }

    [Route("calculate-final-crypto-amount-for-transfer/{cryptoAmount:decimal}")]
    [HttpGet]
    public async Task<IActionResult> CalculateFinalCryptoAmountForTransfer(decimal cryptoAmount)
    {
        var result = await _mediator.Send(new CalculateFinalCryptoAmountForTransferCommand { CryptoAmount = cryptoAmount });

        return Web.Response.Create200(result);
    }
    
    [Route("get-exchanger-account-address")]
    [HttpGet]
    public async Task<IActionResult> GetExchangerAccountAddress()
    {
        var result = await _mediator.Send(new GetExchangerAccountAddressCommand());

        return Web.Response.Create200(result);
    }

    private readonly IMediator _mediator;
}
