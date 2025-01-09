using Core.Application.BuyOrder.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[Route("api/buy-order")]
[Authorize]
public class BuyOrderApiController : Controller
{
    public BuyOrderApiController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [Route("create")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBuyOrderCommand? request)
    {
        if (request == null)
            return BadRequest(Web.Response.Fail(new { message = "Request format is invalid." }).ToJson());
        
        var result = await _mediator.Send(request);

        return ActionResultHelper.CreateResponse(result, HttpContext);
    }
    
    [Route("respond-by-seller")]
    [HttpPost]
    public async Task<IActionResult> RespondBySeller([FromBody] RespondToBuyOrderBySellerCommand? request)
    {
        if (request == null)
            return BadRequest(Web.Response.Fail(new { message = "Request format is invalid." }).ToJson());
        
        var result = await _mediator.Send(request);

        return ActionResultHelper.CreateResponse(result, HttpContext);
    }
    
    [Route("confirm-transfer-fiat-to-seller-by-buyer")]
    [HttpPost]
    public async Task<IActionResult> ConfirmTransferFiatToSellerByBuyer([FromBody] ConfirmTransferFiatToSellerByBuyerForBuyOrderCommand? request)
    {
        if (request == null)
            return BadRequest(Web.Response.Fail(new { message = "Request format is invalid." }).ToJson());
        
        var result = await _mediator.Send(request);

        return ActionResultHelper.CreateResponse(result, HttpContext);
    }
    
    [Route("confirm-receipt-fiat-from-buyer-by-seller")]
    [HttpPost]
    public async Task<IActionResult> ConfirmReceiptFiatFromBuyerBySeller([FromBody] ConfirmReceiptFiatFromBuyerBySellerForBuyOrderCommand? request)
    {
        if (request == null)
            return BadRequest(Web.Response.Fail(new { message = "Request format is invalid." }).ToJson());
        
        var result = await _mediator.Send(request);

        return ActionResultHelper.CreateResponse(result, HttpContext);
    }
    
    private readonly IMediator _mediator;
}