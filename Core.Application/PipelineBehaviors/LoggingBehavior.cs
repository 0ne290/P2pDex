using System.Diagnostics;
using Core.Application.Errors;
using Core.Domain.Exceptions;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.PipelineBehaviors;

public class LoggingBehavior<TResponse> : IPipelineBehavior<IRequest<Result<TResponse>>, Result<TResponse>>
{
    public LoggingBehavior(ILogger<LoggingBehavior<TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<Result<TResponse>> Handle(IRequest<Result<TResponse>> request, RequestHandlerDelegate<Result<TResponse>> next,
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
                "SUCCESSFUL END", requestName, requestGuid, request, stopwatch.ElapsedMilliseconds, response);

            return response;
        }
        catch (InvariantViolationException e)
        {
            stopwatch.Stop();
            _logger.LogInformation(
                "Stage: {stage}, request name: {requestName}, request GUID: {requestGuid}, request body: {@requestBody}, message: {message}.",
                "FAILED END", requestName, requestGuid, request, e.Message);

            return Result.Fail(e.Message);
        }
        catch (DevelopmentErrorException e)
        {
            stopwatch.Stop();
            _logger.LogError(
                "Stage: {stage}, request name: {requestName}, request GUID: {requestGuid}, request body: {@requestBody}, error detail: {@errorDetail}.",
                "KNOWN ERROR", requestName, requestGuid, request, e);

            return Result.Fail(
                new DevelopmentError().WithMetadata(
                    new Dictionary<string, object>(
                        [new KeyValuePair<string, object>(nameof(requestGuid), requestGuid)])));
        }
        catch (Exception e)
        {
            stopwatch.Stop();
            _logger.LogError(
                "Stage: {stage}, request name: {requestName}, request GUID: {requestGuid}, request body: {@requestBody}, error detail: {@errorDetail}.",
                "UNKNOWN ERROR", requestName, requestGuid, request, e);

            return Result.Fail(
                new DevelopmentError().WithMetadata(
                    new Dictionary<string, object>(
                        [new KeyValuePair<string, object>(nameof(requestGuid), requestGuid)])));
        }
    }

    private readonly ILogger<LoggingBehavior<TResponse>> _logger;
}