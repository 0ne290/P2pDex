using System.Diagnostics;
using Core.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Web.Filters;

public class ExceptionHandlerAndLoggerFilter : IAsyncActionFilter
{
    public ExceptionHandlerAndLoggerFilter(ILogger<ExceptionHandlerAndLoggerFilter> logger)
    {
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext actionExecutingContext,
        ActionExecutionDelegate next)
    {
        var controllerActionDescriptor = (ControllerActionDescriptor)actionExecutingContext.ActionDescriptor;

        var requestName = controllerActionDescriptor.DisplayName!;
        var requestGuid = Guid.NewGuid().ToString();

        _logger.LogInformation(
            "Stage: {stage}, request name: {requestName}, request GUID: {requestGuid}, request body: {@requestBody}.",
            "START",
            requestName, requestGuid, actionExecutingContext.ActionArguments);

        var stopwatch = Stopwatch.StartNew();
        var actionExecutedContext = await next();
        stopwatch.Stop();
        actionExecutedContext.ExceptionHandled = true;// Если закомментировать эту строку, подменить ответ клиенту не получится, т. к. любой фильтр исключений будет переопределять мою подмену. Т. е. если ExceptionHandled = true, цепочка вызовов фильтров исключений прекращается.

        var exception = actionExecutedContext.Exception;
        switch (exception)
        {
            case null:
                _logger.LogInformation(
                    "Stage: {stage}, request GUID: {requestGuid}, execution time in ms: {executionTime}, response: {@response}.",
                    "RETURN 200", requestGuid, stopwatch.ElapsedMilliseconds, actionExecutedContext.Result);
                
                break;
            
            case InvariantViolationException:
                _logger.LogInformation(
                    "Stage: {stage}, request GUID: {requestGuid}, message: {message}.", "RETURN 400", requestGuid, exception.Message);

                actionExecutedContext.Result = ResponseCreator.Create(new Error400 { Message = exception.Message } );
                
                break;
            
            default:
                _logger.LogError(
                    "Stage: {stage}, request GUID: {requestGuid}, error detail: {@errorDetail}.",
                    "RETURN 500", requestGuid, exception);

                actionExecutedContext.Result = ResponseCreator.Create(new Error500
                {
                    RequestName = requestName, RequestGuid = requestGuid,
                    TraceId = actionExecutingContext.HttpContext.TraceIdentifier
                });
                
                break;
        }
    }

    private readonly ILogger<ExceptionHandlerAndLoggerFilter> _logger;
}