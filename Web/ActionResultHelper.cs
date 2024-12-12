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
    
    private static OkObjectResult Create200(CommandResult result) =>
        new(Response.Success(result.Value).ToJson());

    private static BadRequestObjectResult Create400(CommandResult result) =>
        new(Response.Fail(new { message = result.Errors[0].Message }).ToJson());

    private static ObjectResult Create500(CommandResult result, HttpContext httpContext) => new(new
    {
        message = "Please report the issue to technical support and attach this response body to your message.",
        requestGuid = result.RequestGuid.ToString(), requestName = result.RequestName,
        url = httpContext.Request.GetEncodedUrl(), traceId = httpContext.TraceIdentifier
    }) { StatusCode = 500 };
}