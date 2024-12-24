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
    public async Task<IActionResult> CreateSellOrder([FromBody] CreateSellOrderCommand? request)
    {
        if (request == null)
            return BadRequest(Web.Response.Fail(new { message = "Request format is invalid." }).ToJson());
        
        var result = await _mediator.Send(request);

        return ActionResultHelper.CreateResponse(result, HttpContext);
    }
    
    [Route("respond-to-sell-order-by-buyer")]
    [HttpPost]
    public async Task<IActionResult> RespondToSellOrderByBuyer([FromBody] RespondToSellOrderByBuyerCommand? request)
    {
        if (request == null)
            return BadRequest(Web.Response.Fail(new { message = "Request format is invalid." }).ToJson());
        
        var result = await _mediator.Send(request);

        return ActionResultHelper.CreateResponse(result, HttpContext);
    }
    
    [Route("confirm-transfer-fiat-to-seller-by-buyer-for-sell-order")]
    [HttpPost]
    public async Task<IActionResult> ConfirmTransferFiatToSellerByBuyerForSellOrder([FromBody] ConfirmTransferFiatToSellerByBuyerForSellOrderCommand? request)
    {
        if (request == null)
            return BadRequest(Web.Response.Fail(new { message = "Request format is invalid." }).ToJson());
        
        var result = await _mediator.Send(request);

        return ActionResultHelper.CreateResponse(result, HttpContext);
    }
    
    [Route("confirm-receipt-fiat-from-buyer-by-seller-for-sell-order")]
    [HttpPost]
    public async Task<IActionResult> ConfirmReceiptFiatFromBuyerBySellerForSellOrder([FromBody] ConfirmReceiptFiatFromBuyerBySellerForSellOrderCommand? request)
    {
        if (request == null)
            return BadRequest(Web.Response.Fail(new { message = "Request format is invalid." }).ToJson());
        
        var result = await _mediator.Send(request);

        return ActionResultHelper.CreateResponse(result, HttpContext);
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
