using System.ComponentModel;
using Core.Application.Api.General.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[Route("api")]
[ApiController]
public class GeneralApiController : Controller
{
    public GeneralApiController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [EndpointSummary("Гарантирует существование трейдера.")]
    [EndpointDescription("Если трейдер уже существует, он будет либо обновлен, если это требуется, либо никаких" +
                         " действий не будет выполнено. Если трейдер не существует, он будет создан.")]
    [ProducesResponseType<Response<EnsureExistedOfTraderResponse>>(StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType<Response<Error400>>(StatusCodes.Status400BadRequest, "application/json")]
    [ProducesResponseType<Response<Error500>>(StatusCodes.Status500InternalServerError, "application/json")]
    [Route("ensure-existed-of-trader/{userId:long}:{userName}")]
    [HttpGet]
    public async Task<Response<EnsureExistedOfTraderResponse>> EnsureExistedOfTrader([Description("Идентификатор трейдера.")] long userId, [Description("Имя трейдера.")] string userName)
    {
        var result = await _mediator.Send(new EnsureExistedOfTraderCommand
            { Id = userId, Name = userName == "null" ? null : userName });

        return ResponseCreator.Create(result);
    }

    [EndpointSummary("Вычисляет общую сумму криптовалюты для создания заказа.")]
    [EndpointDescription("Сумма вычисляется по формуле " +
                         "\"Продаваемая сумма\" + \"Средняя приоритетная комиссия\" + \"Ожидаемая базовая комиссия\".")]
    [ProducesResponseType<Response<CalculateFinalCryptoAmountForTransferResponse>>(StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType<Response<Error400>>(StatusCodes.Status400BadRequest, "application/json")]
    [ProducesResponseType<Response<Error500>>(StatusCodes.Status500InternalServerError, "application/json")]
    [Route("calculate-final-crypto-amount-for-transfer/{cryptoAmount:decimal}")]
    [HttpGet]
    public async Task<Response<CalculateFinalCryptoAmountForTransferResponse>> CalculateFinalCryptoAmountForTransfer([Description("Продаваемая сумма.")] decimal cryptoAmount)
    {
        var result = await _mediator.Send(new CalculateFinalCryptoAmountForTransferCommand { CryptoAmount = cryptoAmount });

        return ResponseCreator.Create(result);
    }
    
    [EndpointSummary("Получает адрес биржи.")]
    [ProducesResponseType<Response<GetExchangerAccountAddressResponse>>(StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType<Response<Error400>>(StatusCodes.Status400BadRequest, "application/json")]
    [ProducesResponseType<Response<Error500>>(StatusCodes.Status500InternalServerError, "application/json")]
    [Route("get-exchanger-account-address")]
    [HttpGet]
    public async Task<Response<GetExchangerAccountAddressResponse>> GetExchangerAccountAddress()
    {
        var result = await _mediator.Send(new GetExchangerAccountAddressCommand());

        return ResponseCreator.Create(result);
    }

    private readonly IMediator _mediator;
}
