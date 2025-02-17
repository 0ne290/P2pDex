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

        var requestName = $"{controllerActionDescriptor.ControllerName}.{controllerActionDescriptor.ActionName}";
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
                var response = (Response)actionExecutedContext.Result!;

                _logger.LogInformation(
                    "Stage: {stage}, request name: {requestName}, request GUID: {requestGuid}, request body: {@requestBody}, execution time in ms: {executionTime}, response: {@response}.",
                    "RETURN 200", requestName, requestGuid, actionExecutingContext.ActionArguments,
                    stopwatch.ElapsedMilliseconds, response.Data);
                
                break;
            
            case InvariantViolationException:
                _logger.LogInformation(
                    "Stage: {stage}, request name: {requestName}, request GUID: {requestGuid}, request body: {@requestBody}, message: {message}.",
                    "RETURN 400", requestName, requestGuid, actionExecutingContext.ActionArguments, exception.Message);

                actionExecutedContext.Result = Response.Create400(exception.Message);
                
                break;
            
            default:
                _logger.LogError(
                    "Stage: {stage}, request name: {requestName}, request GUID: {requestGuid}, request body: {@requestBody}, error detail: {@errorDetail}.",
                    "RETURN 500", requestName, requestGuid, actionExecutingContext.ActionArguments, exception);
            
                actionExecutedContext.Result = Response.Create500(requestName, requestGuid, actionExecutingContext.HttpContext);
                
                break;
        }
    }

    private readonly ILogger<ExceptionHandlerAndLoggerFilter> _logger;
}