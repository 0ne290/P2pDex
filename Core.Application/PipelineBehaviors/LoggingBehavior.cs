using System.Diagnostics;
using System.Text.Json;
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

        _logger.LogInformation("Stage: {stage}, request name: {requestName}, request body: {@requestBody}.", "START",
            requestName, request);

        var stopwatch = Stopwatch.StartNew();
        try
        {
            var response = await next();
            stopwatch.Stop();
            
            _logger.LogInformation(
                "Stage: {stage}, request name: {requestName}, request body: {@requestBody}, execution time in ms: {executionTime}, response: {@response}.",
                "END", requestName, request, stopwatch.ElapsedMilliseconds, response);

            return response;
        }
        catch (Exception e)
        {
            stopwatch.Stop();
            _logger.LogError(
                "Stage: {stage}, request name: {requestName}, request body: {@requestBody}, error detail: {@errorDetail}.",
                "ERROR", requestName, request, e);

            return Result.Fail("sg");// TODO: Выделить данную ошибку в отдельный тип для простоты ее идентификации. Генерировать для каждой приходящей команды GUID и добавлять его к инфе об ошибке. Контроллеры ASP.NET Core, получающие такие ошибки, должны формировать свои собственные сообщения для клиента, взяв за основу сообщение полученной ошибки и добавив к ней контекстную инфу, содержащуюся в логах (например, время, HttpContext.TraceIdentifier, URL и т. д.), для облегчения поиска нужных логов.
        }
    }

    private readonly ILogger<TRequest> _logger;
}