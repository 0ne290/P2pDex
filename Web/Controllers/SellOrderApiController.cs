using System.ComponentModel;
using Core.Application.Api.SellOrder;
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
    [ProducesResponseType<Response<GetAllSellOrdersResponse>>(StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType<Response<Error400>>(StatusCodes.Status400BadRequest, "application/json")]
    [ProducesResponseType<Response<Error500>>(StatusCodes.Status500InternalServerError, "application/json")]
    [Route("get-all/{traderId:long}")]
    [HttpGet]
    public async Task<Response<GetAllSellOrdersResponse>> GetAll(
        [Description("Идентификатор трейдера, от имени которого выполняется запрос.")]
        long traderId
        )
    {
        var result = await _mediator.Send(new GetAllSellOrdersCommand { TraderId = traderId });

        return ResponseCreator.Create(result);
    }
    
    [EndpointSummary("Получает заказ.")]
    [EndpointDescription("Получает заказ вместе с его покупателем и продавцом. Если трейдера или заказа не существует," +
                         " возвращается 400.")]
    [ProducesResponseType<Response<GetSellOrderResponse>>(StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType<Response<Error400>>(StatusCodes.Status400BadRequest, "application/json")]
    [ProducesResponseType<Response<Error500>>(StatusCodes.Status500InternalServerError, "application/json")]
    [Route("get/{traderId:long}:{orderGuid:guid}")]
    [HttpGet]
    public async Task<Response<GetSellOrderResponse>> Get(
        [Description("Идентификатор трейдера, от имени которого выполняется запрос.")]
        long traderId,
        [Description("Идентификатор заказа.")]
        Guid orderGuid)
    {
        var result = await _mediator.Send(new GetSellOrderCommand { TraderId = traderId, OrderGuid = orderGuid });

        return ResponseCreator.Create(result);
    }
    
    [EndpointSummary("Создает заказ.")]
    [EndpointDescription("Статус созданного заказа - \"Created\".")]
    [ProducesResponseType<Response<OrderStatusChangeResponse>>(StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType<Response<Error400>>(StatusCodes.Status400BadRequest, "application/json")]
    [ProducesResponseType<Response<Error500>>(StatusCodes.Status500InternalServerError, "application/json")]
    [Route("create")]
    [HttpPost]
    public async Task<Response<OrderStatusChangeResponse>> Create([FromBody] CreateSellOrderCommand request)
    {
        var result = await _mediator.Send(request);

        return ResponseCreator.Create(result);
    }
    
    [EndpointSummary("Откликается на заказ в качестве покупателя.")]
    [EndpointDescription("Устанавливает статус заказа в \"RespondedByBuyer\". Статус заказа должен быть" +
                         " \"SellerToExchangerTransferTransactionConfirmed\". Этот статус приложение должно" +
                         " устанавливать само каждые 2 минуты всем заказам в статусе \"Created\", чьи транзакции были" +
                         " подтверждены.")]
    [ProducesResponseType<Response<OrderStatusChangeResponse>>(StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType<Response<Error400>>(StatusCodes.Status400BadRequest, "application/json")]
    [ProducesResponseType<Response<Error500>>(StatusCodes.Status500InternalServerError, "application/json")]
    [Route("respond-by-buyer")]
    [HttpPost]
    public async Task<Response<OrderStatusChangeResponse>> RespondByBuyer([FromBody] RespondToSellOrderByBuyerCommand request)
    {
        var result = await _mediator.Send(request);

        await _sellOrderHub.PublishAStatusChangeNotification(result.Guid.ToString(), result.Status.ToString());

        return ResponseCreator.Create(result);
    }
    
    [EndpointSummary("Подтверждает перевод фиата покупателем продавцу.")]
    [EndpointDescription("Устанавливает статус заказа в \"TransferFiatToSellerConfirmedByBuyer\". Статус заказа должен" +
                         " быть \"RespondedByBuyer\".")]
    [ProducesResponseType<Response<OrderStatusChangeResponse>>(StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType<Response<Error400>>(StatusCodes.Status400BadRequest, "application/json")]
    [ProducesResponseType<Response<Error500>>(StatusCodes.Status500InternalServerError, "application/json")]
    [Route("confirm-transfer-fiat-to-seller-by-buyer")]
    [HttpPost]
    public async Task<Response<OrderStatusChangeResponse>> ConfirmTransferFiatToSellerByBuyer([FromBody] ConfirmTransferFiatToSellerByBuyerForSellOrderCommand request)
    {
        var result = await _mediator.Send(request);
        
        await _sellOrderHub.PublishAStatusChangeNotification(result.Guid.ToString(), result.Status.ToString());

        return ResponseCreator.Create(result);
    }
    
    [EndpointSummary("Подтверждает получение продавцом фиата покупателя.")]
    [EndpointDescription("Устанавливает статус заказа в \"ReceiptFiatFromBuyerConfirmedBySeller\". Статус заказа должен" +
                         " быть \"TransferFiatToSellerConfirmedByBuyer\". Каждые 2 минуты приложение должно переводить" +
                         " все заказы в статусе \"ReceiptFiatFromBuyerConfirmedBySeller\", чьи транзакции были" +
                         " подтверждены, в конечный статус \"ExchangerToBuyerTransferTransactionConfirmed\"," +
                         " означающий, что заказ успешно завершен.")]
    [ProducesResponseType<Response<OrderStatusChangeResponse>>(StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType<Response<Error400>>(StatusCodes.Status400BadRequest, "application/json")]
    [ProducesResponseType<Response<Error500>>(StatusCodes.Status500InternalServerError, "application/json")]
    [Route("confirm-receipt-fiat-from-buyer-by-seller")]
    [HttpPost]
    public async Task<Response<OrderStatusChangeResponse>> ConfirmReceiptFiatFromBuyerBySeller([FromBody] ConfirmReceiptFiatFromBuyerBySellerForSellOrderCommand request)
    {
        var result = await _mediator.Send(request);
        
        await _sellOrderHub.PublishAStatusChangeNotification(result.Guid.ToString(), result.Status.ToString());

        return ResponseCreator.Create(result);
    }
    
    private readonly IMediator _mediator;

    private readonly IHubContext<SellOrderHub> _sellOrderHub;
}