using Core.Application.Api.SellOrder.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Web.Hubs;

namespace Web.Controllers;

[Route("api/sell-order")]
public class SellOrderApiController : Controller
{
    public SellOrderApiController(IMediator mediator, IHubContext<SellOrderHub> sellOrderHub)
    {
        _mediator = mediator;
        _sellOrderHub = sellOrderHub;
    }
    
    [Route("get-all")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllSellOrdersCommand());

        return Web.Response.Create200(result);
    }
    
    [Route("create")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSellOrderCommand request)
    {
        var result = await _mediator.Send(request);

        return Web.Response.Create200(result);
    }
    
    [Route("respond-by-buyer")]
    [HttpPost]
    public async Task<IActionResult> RespondByBuyer([FromBody] RespondToSellOrderByBuyerCommand request)
    {
        var result = await _mediator.Send(request);

        await _sellOrderHub.PublishAStatusChangeNotification(result["guid"].ToString()!, result["status"].ToString()!);

        return Web.Response.Create200(result);
    }
    
    [Route("confirm-transfer-fiat-to-seller-by-buyer")]
    [HttpPost]
    public async Task<IActionResult> ConfirmTransferFiatToSellerByBuyer([FromBody] ConfirmTransferFiatToSellerByBuyerForSellOrderCommand request)
    {
        var result = await _mediator.Send(request);
        
        await _sellOrderHub.PublishAStatusChangeNotification(result["guid"].ToString()!, result["status"].ToString()!);

        return Web.Response.Create200(result);
    }
    
    [Route("confirm-receipt-fiat-from-buyer-by-seller")]
    [HttpPost]
    public async Task<IActionResult> ConfirmReceiptFiatFromBuyerBySeller([FromBody] ConfirmReceiptFiatFromBuyerBySellerForSellOrderCommand request)
    {
        var result = await _mediator.Send(request);
        
        await _sellOrderHub.PublishAStatusChangeNotification(result["guid"].ToString()!, result["status"].ToString()!);

        return Web.Response.Create200(result);
    }
    
    private readonly IMediator _mediator;

    private readonly IHubContext<SellOrderHub> _sellOrderHub;
}