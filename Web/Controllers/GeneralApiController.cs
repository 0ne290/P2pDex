using Core.Application.General.Commands;
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
        var result = await _mediator.Send(new EnsureExistedOfTraderCommand { Id = userId, Name = userName});

        return ActionResultHelper.CreateResponse(result, HttpContext);
    }

    [Route("calculate-final-crypto-amount-for-transfer/{cryptoAmount:decimal}")]
    [HttpGet]
    public async Task<IActionResult> CalculateFinalCryptoAmountForTransfer(decimal cryptoAmount)
    {
        var result = await _mediator.Send(new CalculateFinalCryptoAmountForTransferCommand { CryptoAmount = cryptoAmount });

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
