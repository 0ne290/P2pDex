using System.Diagnostics;
using Core.Application.Errors;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.PipelineBehaviors;

public class LoggingBehavior<TRequest> : IPipelineBehavior<TRequest, IResultBase> where TRequest : notnull
{
    public LoggingBehavior(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    public async Task<IResultBase> Handle(TRequest request, RequestHandlerDelegate<IResultBase> next,
        CancellationToken cancellationToken)
    {
        var requestName = request.GetType().Name;
        var requestGuid = Guid.NewGuid().ToString();

        _logger.LogInformation("Stage: {stage}, request name: {requestName}, request GUID: {requestGuid}, request body: {@requestBody}.", "START",
            requestName, requestGuid, request);

        var stopwatch = Stopwatch.StartNew();
        try
        {
            var response = await next();
            stopwatch.Stop();
            
            _logger.LogInformation(
                "Stage: {stage}, request name: {requestName}, request GUID: {requestGuid}, request body: {@requestBody}, execution time in ms: {executionTime}, response: {@response}.",
                "END", requestName, requestGuid, request, stopwatch.ElapsedMilliseconds, response);

            return response;
        }
        catch (Exception e)
        {
            stopwatch.Stop();
            _logger.LogError(
                "Stage: {stage}, request name: {requestName}, request GUID: {requestGuid}, request body: {@requestBody}, error detail: {@errorDetail}.",
                "ERROR", requestName, requestGuid, request, e);

            return Result.Fail(
                new DevelopmentError().WithMetadata(
                    new Dictionary<string, object>(
                        [new KeyValuePair<string, object>(nameof(requestGuid), requestGuid)])));
        }
    }

    private readonly ILogger<TRequest> _logger;
}