using System.Diagnostics;
using Core.Domain.Exceptions;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse> 
    where TResponse : CommandResult, new()
{
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken _)
    {
        var requestName = request.GetType().Name;
        var requestGuid = Guid.NewGuid();
        var requestGuidString = requestGuid.ToString();

        _logger.LogInformation("Stage: {stage}, request name: {requestName}, request GUID: {requestGuid}, request body: {@requestBody}.", "START",
            requestName, requestGuidString, request);

        var stopwatch = Stopwatch.StartNew();
        try
        {
            var response = await next();
            stopwatch.Stop();

            response.RequestGuid = requestGuid;
            response.RequestName = requestName;
            
            _logger.LogInformation(
                "Stage: {stage}, request name: {requestName}, request GUID: {requestGuid}, request body: {@requestBody}, execution time in ms: {executionTime}, response: {@response}.",
                "SUCCESSFUL END", requestName, requestGuidString, request, stopwatch.ElapsedMilliseconds, response);

            return response;
        }
        catch (InvariantViolationException e)
        {
            stopwatch.Stop();
            _logger.LogInformation(
                "Stage: {stage}, request name: {requestName}, request GUID: {requestGuid}, request body: {@requestBody}, message: {message}.",
                "FAILED END", requestName, requestGuidString, request, e.Message);

            var response = new TResponse
            {
                RequestGuid = requestGuid,
                RequestName = requestName
            };
            response.Reasons.Add(new Error(e.Message));

            return response;
        }
        catch (Exception e)
        {
            stopwatch.Stop();
            _logger.LogError(
                "Stage: {stage}, request name: {requestName}, request GUID: {requestGuid}, request body: {@requestBody}, error detail: {@errorDetail}.",
                "ERROR", requestName, requestGuidString, request, e);
            
            var response = new TResponse
            {
                RequestGuid = requestGuid,
                RequestName = requestName
            };
            response.Reasons.Add(new DevelopmentError());

            return response;
        }
    }

    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
}