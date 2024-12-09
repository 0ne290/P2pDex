using Core.Application.Commands;
using Core.Domain.Interfaces;
using FluentResults;
using MediatR;

namespace Core.Application.Handlers;

public class GetTransferTransactionFeeHandler : IRequestHandler<GetTransferTransactionFeeCommand,
    Result<(decimal Value, double TimeToUpdateInMs)>>
{
    public GetTransferTransactionFeeHandler(IBlockchain blockchain)
    {
        _blockchain = blockchain;
    }

    public Task<Result<(decimal Value, double TimeToUpdateInMs)>> Handle(GetTransferTransactionFeeCommand _,
        CancellationToken __) => Task.FromResult(Result.Ok(_blockchain.TransferTransactionFee));

    private readonly IBlockchain _blockchain;
}