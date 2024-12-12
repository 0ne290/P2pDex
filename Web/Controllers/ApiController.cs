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
    public async Task<IActionResult> CreateSellOrder([FromBody] CreateSellOrderCommand request)
    {
        var result = await _mediator.Send(request);

        return ActionResultHelper.CreateResponse(result, HttpContext);
    }
    
    [Route("create-trader")]
    [HttpPost]
    public async Task<IActionResult> CreateTrader([FromBody] CreateTraderCommand request)
    {
        var result = await _mediator.Send(request);

        return ActionResultHelper.CreateResponse(result, HttpContext);
    }
    
    [Route("get-transfer-transaction-fee")]
    [HttpGet]
    public async Task<IActionResult> GetTransferTransactionFee()
    {
        var result = await _mediator.Send(new GetTransferTransactionFeeCommand());

        return ActionResultHelper.CreateResponse(result, HttpContext);
    }

    private readonly IMediator _mediator;
}