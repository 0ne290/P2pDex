using Core.Application.Api.SellOrder.Commands;
using Core.Application.Api.SellOrder.Get;
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
    
    [Route("get-all/{traderId:long}")]
    [HttpGet]
    public async Task<IActionResult> GetAll(long traderId)
    {
        var result = await _mediator.Send(new GetAllSellOrdersCommand { TraderId = traderId });

        return Web.Response.Create200(result);
    }
    
    [Route("get/{traderId:long}:{orderGuid:guid}")]
    [HttpGet]
    public async Task<IActionResult> Get(long traderId, Guid orderGuid)
    {
        var result = await _mediator.Send(new GetSellOrderCommand { TraderId = traderId, OrderGuid = orderGuid });

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