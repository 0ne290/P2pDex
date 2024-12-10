using Core.Application.Commands;
using Core.Application.Errors;
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

        if (!result.IsSuccess)
            return result.HasError<DevelopmentError>()
                ? StatusCode(500, Web.Response.Fail(("messages", result.Errors.Select(e => e.Message))).ToJson())
                : BadRequest(Web.Response.Fail(("messages", result.Errors.Select(e => e.Message))).ToJson());
        
        var resultValue = result.Value;
            
        return Ok(Web.Response.Success(("guid", resultValue.Item1), ("status", resultValue.Item2)).ToJson());

    }
    
    [Route("create-trader")]
    [HttpPost]
    public async Task<IActionResult> CreateTrader([FromBody] CreateTraderCommand request)
    {
        var result = await _mediator.Send(request);

        if (!result.IsSuccess)
            return result.HasError<DevelopmentError>()
                ? StatusCode(500, Web.Response.Fail(("messages", result.Errors.Select(e => e.Message))).ToJson())
                : BadRequest(Web.Response.Fail(("messages", result.Errors.Select(e => e.Message))).ToJson());
        
        return Ok(Web.Response.Success(("guid", result.Value)).ToJson());
    }
    
    [Route("get-transfer-transaction-fee")]
    [HttpGet]
    public async Task<IActionResult> GetTransferTransactionFee()
    {
        var result = await _mediator.Send(new GetTransferTransactionFeeCommand());

        if (!result.IsSuccess)
            return result.HasError<DevelopmentError>()
                ? StatusCode(500, Web.Response.Fail(("messages", result.Errors.Select(e => e.Message))).ToJson())
                : BadRequest(Web.Response.Fail(("messages", result.Errors.Select(e => e.Message))).ToJson());
        
        var resultValue = result.Value;
            
        return Ok(Web.Response.Success(("fee", resultValue.Item1), ("timeToUpdateInMs", resultValue.Item2)).ToJson());

    }

    private readonly IMediator _mediator;
}