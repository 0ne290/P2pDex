using Core.Application.Commands;
using Core.Domain.Interfaces;
using MediatR;

namespace Core.Application.Handlers;

public class GetTransferTransactionFeeHandler : IRequestHandler<GetTransferTransactionFeeCommand, CommandResult>
{
    public GetTransferTransactionFeeHandler(IBlockchain blockchain)
    {
        _blockchain = blockchain;
    }

    public Task<CommandResult> Handle(GetTransferTransactionFeeCommand _, CancellationToken __)
    {
        var ret = _blockchain.TransferTransactionFee;

        return Task.FromResult(new CommandResult(new
            { fee = ret.Value, timeToUpdateInMs = ret.TimeToUpdateInMs }));
    }
        

    private readonly IBlockchain _blockchain;
}