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
    
    public async Task OnActionExecutionAsync(ActionExecutingContext actionExecutingContext, ActionExecutionDelegate next)
    {
        var controllerActionDescriptor = (ControllerActionDescriptor)actionExecutingContext.ActionDescriptor;
        
        var requestName = $"{controllerActionDescriptor.ControllerName}.{controllerActionDescriptor.ActionName}";
        var requestGuid = Guid.NewGuid().ToString();

        _logger.LogInformation("Stage: {stage}, request name: {requestName}, request GUID: {requestGuid}, request body: {@requestBody}.", "START",
            requestName, requestGuid, actionExecutingContext.ActionArguments);

        var stopwatch = Stopwatch.StartNew();
        try
        {
            var actionExecutedContext = await next();
            stopwatch.Stop();

            var response = (Response)actionExecutedContext.Result!;

            _logger.LogInformation(
                "Stage: {stage}, request name: {requestName}, request GUID: {requestGuid}, request body: {@requestBody}, execution time in ms: {executionTime}, response: {@response}.",
                "RETURN 200", requestName, requestGuid, actionExecutingContext.ActionArguments, stopwatch.ElapsedMilliseconds, response.Data);
        }
        catch (InvariantViolationException e)
        {
            stopwatch.Stop();
            _logger.LogInformation(
                "Stage: {stage}, request name: {requestName}, request GUID: {requestGuid}, request body: {@requestBody}, message: {message}.",
                "RETURN 400", requestName, requestGuid, actionExecutingContext.ActionArguments, e.Message);

            actionExecutingContext.Result = Response.Create400(e.Message);
        }
        catch (Exception e)
        {
            stopwatch.Stop();
            _logger.LogError(
                "Stage: {stage}, request name: {requestName}, request GUID: {requestGuid}, request body: {@requestBody}, error detail: {@errorDetail}.",
                "RETURN 500", requestName, requestGuid, actionExecutingContext.ActionArguments, e);
            
            actionExecutingContext.Result = Response.Create500(requestName, requestGuid, actionExecutingContext.HttpContext);
        }
    }
    
    private readonly ILogger<ExceptionHandlerAndLoggerFilter> _logger;
}