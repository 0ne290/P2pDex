using Core.Application.SellOrder.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[Route("api/sell-order")]
public class SellOrderApiController : Controller
{
    public SellOrderApiController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [Route("create")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSellOrderCommand? request)
    {
        if (request == null)
            return BadRequest(Web.Response.Fail(new { message = "Request format is invalid." }).ToJson());
        
        var result = await _mediator.Send(request);

        return ActionResultHelper.CreateResponse(result, HttpContext);
    }
    
    [Route("respond-by-buyer")]
    [HttpPost]
    public async Task<IActionResult> RespondByBuyer([FromBody] RespondToSellOrderByBuyerCommand? request)
    {
        if (request == null)
            return BadRequest(Web.Response.Fail(new { message = "Request format is invalid." }).ToJson());
        
        var result = await _mediator.Send(request);

        return ActionResultHelper.CreateResponse(result, HttpContext);
    }
    
    [Route("confirm-transfer-fiat-to-seller-by-buyer")]
    [HttpPost]
    public async Task<IActionResult> ConfirmTransferFiatToSellerByBuyer([FromBody] ConfirmTransferFiatToSellerByBuyerForSellOrderCommand? request)
    {
        if (request == null)
            return BadRequest(Web.Response.Fail(new { message = "Request format is invalid." }).ToJson());
        
        var result = await _mediator.Send(request);

        return ActionResultHelper.CreateResponse(result, HttpContext);
    }
    
    [Route("confirm-receipt-fiat-from-buyer-by-seller")]
    [HttpPost]
    public async Task<IActionResult> ConfirmReceiptFiatFromBuyerBySeller([FromBody] ConfirmReceiptFiatFromBuyerBySellerForSellOrderCommand? request)
    {
        if (request == null)
            return BadRequest(Web.Response.Fail(new { message = "Request format is invalid." }).ToJson());
        
        var result = await _mediator.Send(request);

        return ActionResultHelper.CreateResponse(result, HttpContext);
    }
    
    private readonly IMediator _mediator;
}