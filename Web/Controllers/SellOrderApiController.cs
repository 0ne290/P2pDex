using System.ComponentModel;
using Core.Application.Api.SellOrder.Commands;
using Core.Application.Api.SellOrder.Get;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Web.Hubs;

namespace Web.Controllers;

[Route("api/sell-order")]
[ApiController]
public class SellOrderApiController : Controller
{
    public SellOrderApiController(IMediator mediator, IHubContext<SellOrderHub> sellOrderHub)
    {
        _mediator = mediator;
        _sellOrderHub = sellOrderHub;
    }
    
    [EndpointSummary("Получает все заказы.")]
    [EndpointDescription("Получает все заказы со статусом \"SellerToExchangerTransferTransactionConfirmed\". Если" +
                         " трейдера не существует, возвращается 400.")]
    [Route("get-all/{traderId:long}")]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [Description("Идентификатор трейдера, от имени которого выполняется запрос.")]
        long traderId
        )
    {
        var result = await _mediator.Send(new GetAllSellOrdersCommand { TraderId = traderId });

        return Web.Response.Create200(result);
    }
    
    [EndpointSummary("Получает заказ.")]
    [EndpointDescription("Получает заказ вместе с его покупателем и продавцом. Если трейдера или заказа не существует," +
                         " возвращается 400.")]
    [Route("get/{traderId:long}:{orderGuid:guid}")]
    [HttpGet]
    public async Task<IActionResult> Get(
        [Description("Идентификатор трейдера, от имени которого выполняется запрос.")]
        long traderId,
        [Description("Идентификатор заказа.")]
        Guid orderGuid)
    {
        var result = await _mediator.Send(new GetSellOrderCommand { TraderId = traderId, OrderGuid = orderGuid });

        return Web.Response.Create200(result);
    }
    
    [EndpointSummary("Создает заказ.")]
    [EndpointDescription("Статус созданного заказа - \"Created\".")]
    [Route("create")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSellOrderCommand request)
    {
        var result = await _mediator.Send(request);

        return Web.Response.Create200(result);
    }
    
    [EndpointSummary("Откликается на заказ в качестве покупателя.")]
    [EndpointDescription("Устанавливает статус заказа в \"RespondedByBuyer\". Статус заказа должен быть" +
                         " \"SellerToExchangerTransferTransactionConfirmed\". Этот статус приложение должно" +
                         " устанавливать само каждые 2 минуты всем заказам в статусе \"Created\", чьи транзакции были" +
                         " подтверждены.")]
    [Route("respond-by-buyer")]
    [HttpPost]
    public async Task<IActionResult> RespondByBuyer([FromBody] RespondToSellOrderByBuyerCommand request)
    {
        var result = await _mediator.Send(request);

        await _sellOrderHub.PublishAStatusChangeNotification(result["guid"].ToString()!, result["status"].ToString()!);

        return Web.Response.Create200(result);
    }
    
    [EndpointSummary("Подтверждает перевод фиата покупателем продавцу.")]
    [EndpointDescription("Устанавливает статус заказа в \"TransferFiatToSellerConfirmedByBuyer\". Статус заказа должен" +
                         " быть \"RespondedByBuyer\".")]
    [Route("confirm-transfer-fiat-to-seller-by-buyer")]
    [HttpPost]
    public async Task<IActionResult> ConfirmTransferFiatToSellerByBuyer([FromBody] ConfirmTransferFiatToSellerByBuyerForSellOrderCommand request)
    {
        var result = await _mediator.Send(request);
        
        await _sellOrderHub.PublishAStatusChangeNotification(result["guid"].ToString()!, result["status"].ToString()!);

        return Web.Response.Create200(result);
    }
    
    [EndpointSummary("Подтверждает получение продавцом фиата покупателя.")]
    [EndpointDescription("Устанавливает статус заказа в \"ReceiptFiatFromBuyerConfirmedBySeller\". Статус заказа должен" +
                         " быть \"TransferFiatToSellerConfirmedByBuyer\". Каждые 2 минуты приложение должно переводить" +
                         " все заказы в статусе \"ReceiptFiatFromBuyerConfirmedBySeller\", чьи транзакции были" +
                         " подтверждены, в конечный статус \"ExchangerToBuyerTransferTransactionConfirmed\"," +
                         " означающий, что заказ успешно завершен.")]
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