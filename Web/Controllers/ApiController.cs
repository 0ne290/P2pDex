using Core.Application;
using Core.Application.Commands;
using Core.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[Route("api")]
public class ApiController : Controller
{
    public ApiController(IMediator mediator, IBlockchain blockchain, ExchangerConfiguration exchangerConfiguration)
    {
        _mediator = mediator;
        _blockchain = blockchain;
        _exchangerConfiguration = exchangerConfiguration;
    }

    private readonly IBlockchain _blockchain;

    private readonly ExchangerConfiguration _exchangerConfiguration;
    
    [Route("testing-nonce")]
    [HttpGet]
    public async Task<IActionResult> TestingNonce(string to, decimal amount)
    {
        var transactionHash = await _blockchain.SendTransferTransaction(_exchangerConfiguration.AccountAddress, to, amount);

        return Ok(transactionHash);
    }
    
    [Route("testing-fee")]
    [HttpGet]
    public IActionResult TestingFee()
    {
        var fee = _blockchain.TransferTransactionFee.Value;

        return Ok(fee);
    }
    
    [Route("create-sell-order")]
    [HttpPost]
    public async Task<IActionResult> CreateSellOrder([FromBody] CreateSellOrderCommand? request)
    {
        if (request == null)
            return BadRequest(Web.Response.Fail(new { message = "Request format is invalid." }).ToJson());
        
        var result = await _mediator.Send(request);

        return ActionResultHelper.CreateResponse(result, HttpContext);
    }
    
    [Route("respond-to-sell-order")]
    [HttpPost]
    public async Task<IActionResult> RespondToSellOrder([FromBody] RespondToSellOrderCommand? request)
    {
        if (request == null)
            return BadRequest(Web.Response.Fail(new { message = "Request format is invalid." }).ToJson());
        
        var result = await _mediator.Send(request);

        return ActionResultHelper.CreateResponse(result, HttpContext);
    }
    
    [Route("confirm-order-by-buyer")]
    [HttpPost]
    public async Task<IActionResult> ConfirmOrderByBuyer([FromBody] ConfirmOrderByBuyerCommand? request)
    {
        if (request == null)
            return BadRequest(Web.Response.Fail(new { message = "Request format is invalid." }).ToJson());
        
        var result = await _mediator.Send(request);

        return ActionResultHelper.CreateResponse(result, HttpContext);
    }
    
    [Route("confirm-by-seller-and-complete-order")]
    [HttpPost]
    public async Task<IActionResult> ConfirmBySellerAndCompleteOrder([FromBody] ConfirmBySellerAndCompleteOrderCommand? request)
    {
        if (request == null)
            return BadRequest(Web.Response.Fail(new { message = "Request format is invalid." }).ToJson());
        
        var result = await _mediator.Send(request);

        return ActionResultHelper.CreateResponse(result, HttpContext);
    }
    
    [Route("create-trader")]
    [HttpPost]
    public async Task<IActionResult> CreateTrader([FromBody] CreateTraderCommand? request)
    {
        if (request == null)
            return BadRequest(Web.Response.Fail(new { message = "Request format is invalid." }).ToJson());
        
        var result = await _mediator.Send(request);

        return ActionResultHelper.CreateResponse(result, HttpContext);
    }

    [Route("calculate-final-crypto-amount-for-transfer")]
    [HttpGet]
    public async Task<IActionResult> CalculateFinalCryptoAmountForTransfer(
        [FromBody] CalculateFinalCryptoAmountForTransferCommand? request)
    {
        if (request == null)
            return BadRequest(Web.Response.Fail(new { message = "Request format is invalid." }).ToJson());
        
        var result = await _mediator.Send(request);

        return ActionResultHelper.CreateResponse(result, HttpContext);
    }
    
    [Route("get-exchanger-account-address")]
    [HttpGet]
    public async Task<IActionResult> GetExchangerAccountAddress()
    {
        var result = await _mediator.Send(new GetExchangerAccountAddressCommand());

        return ActionResultHelper.CreateResponse(result, HttpContext);
    }

    private readonly IMediator _mediator;
}