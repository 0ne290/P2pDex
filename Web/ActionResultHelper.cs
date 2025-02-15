using Core.Application;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Web;

public static class ActionResultHelper
{
    public static IActionResult CreateResponse(CommandResult result, HttpContext httpContext)
    {
        if (!result.IsSuccess)
            return result.HasError<DevelopmentError>() ? Create500(result, httpContext) : Create400(result);
        
        return Create200(result);
    }
    
    private static ContentResult Create200(CommandResult result) => new()
    {
        Content = Response.Success(result.Value).ToJson(),
        ContentType = "application/json; charset=utf-8",
        StatusCode = 200
    };

    private static ContentResult Create400(CommandResult result) => new()
    {
        Content = Response.Fail(new { message = result.Errors[0].Message }).ToJson(),
        ContentType = "application/json; charset=utf-8",
        StatusCode = 400
    };

    private static ContentResult Create500(CommandResult result, HttpContext httpContext) => new()
    {
        Content =
            $"{{\"message\":\"Please report the issue to technical support and attach this response body to your message.\",\"requestGuid\":\"{result.RequestGuid.ToString()}\",\"requestName\":\"{result.RequestName}\",\"url\":\"{httpContext.Request.GetEncodedUrl()}\",\"traceId\":\"{httpContext.TraceIdentifier}\"}}",
        ContentType = "application/json; charset=utf-8",
        StatusCode = 500
    };
}