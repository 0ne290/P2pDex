using System.ComponentModel;
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
    
    [EndpointSummary("Гарантирует существование трейдера.")]
    [EndpointDescription("Если трейдер уже существует, он будет либо обновлен, если это требуется, либо никаких" +
                         " действий не будет выполнено. Если трейдер не существует, он будет создан.")]
    [Route("ensure-existed-of-trader/{userId:long}:{userName}")]
    [HttpGet]
    public async Task<IActionResult> EnsureExistedOfTrader([Description("Идентификатор трейдера.")] long userId, [Description("Имя трейдера.")] string userName)
    {
        var result = await _mediator.Send(new EnsureExistedOfTraderCommand
            { Id = userId, Name = userName == "null" ? null : userName });

        return Web.Response.Create200(result);
    }

    [EndpointSummary("Вычисляет общую сумму криптовалюты для создания заказа.")]
    [EndpointDescription("Сумма вычисляется по формуле " +
                         "\"Продаваемая сумма\" + \"Средняя приоритетная комиссия\" + \"Ожидаемая базовая комиссия\".")]
    [Route("calculate-final-crypto-amount-for-transfer/{cryptoAmount:decimal}")]
    [HttpGet]
    public async Task<IActionResult> CalculateFinalCryptoAmountForTransfer([Description("Продаваемая сумма.")] decimal cryptoAmount)
    {
        var result = await _mediator.Send(new CalculateFinalCryptoAmountForTransferCommand { CryptoAmount = cryptoAmount });

        return Web.Response.Create200(result);
    }
    
    [EndpointSummary("Получает адрес биржи.")]
    [Route("get-exchanger-account-address")]
    [HttpGet]
    public async Task<IActionResult> GetExchangerAccountAddress()
    {
        var result = await _mediator.Send(new GetExchangerAccountAddressCommand());

        return Web.Response.Create200(result);
    }

    private readonly IMediator _mediator;
}
