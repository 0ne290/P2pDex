using Core.Application;
using Core.Application.Commands;
using Core.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace Web.Controllers;

[Route("api")]
public class ApiController : Controller
{
    public ApiController(IMediator mediator, IBlockchain blockchain, Web3 web3, ExchangerConfiguration exchangerConfiguration)
    {
        _mediator = mediator;
        _blockchain = blockchain;
        _web3 = web3;
        _exchangerConfiguration = exchangerConfiguration;
    }

    private readonly IBlockchain _blockchain;

    private readonly Web3 _web3;

    private readonly ExchangerConfiguration _exchangerConfiguration;

    private int _synchronizer = 0;
    
    [Route("testing-nonce")]
    [HttpGet]
    public async Task<IActionResult> TestingNonce(string to, decimal amount)
    {
        Interlocked.Increment(ref _synchronizer);
        
        Console.WriteLine($"{_synchronizer} begin");
        
        while (_synchronizer < 10)
            Thread.Yield();
        
        var transactionHash = await _blockchain.SendTransferTransaction(_exchangerConfiguration.AccountAddress, to, amount);

        var transaction = await _web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(transactionHash);
        
        Console.WriteLine($"{_synchronizer} end");
        
        Interlocked.Decrement(ref _synchronizer);
        
        while (_synchronizer != 0)
            Thread.Yield();
        
        return Ok(transaction);
    }
    
    [Route("testing-transaction-count")]
    [HttpGet]
    public async Task<IActionResult> TestingTransactionCount()
    {
        var x =
            (int)(await _web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(
                _web3.TransactionManager.Account.Address, BlockParameter.CreatePending())).Value;
        var c =
            (int)(await _web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(
                _web3.TransactionManager.Account.Address, BlockParameter.CreateLatest())).Value;
        //var v =
        //    (int)(await _web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(
        //        _web3.TransactionManager.Account.Address, BlockParameter.CreateEarliest())).Value;

        return Ok(new { pending = x, latest = c });
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