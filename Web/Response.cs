using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Web;

public class Response : ContentResult
{
    private Response(IDictionary<string, object> data, int statusCode)
    {
        Data = data;

        Content = JsonConvert.SerializeObject(data);
        ContentType = "application/json; charset=utf-8";
        StatusCode = statusCode;
    }
    
    public static Response Create200(IDictionary<string, object> data) => new(data, 200);

    public static Response Create400(string message) =>
        new(new Dictionary<string, object> { ["message"] = message }, 400);

    public static Response Create500(string requestName, string requestGuid, HttpContext httpContext) => new(
        new Dictionary<string, object>
        {
            ["message"] = "Please report the issue to technical support and attach this response body to your message",
            ["requestGuid"] = requestGuid, ["requestName"] = requestName, ["url"] = httpContext.Request.GetEncodedUrl(),
            ["traceId"] = httpContext.TraceIdentifier
        }, 500);
    
    public IDictionary<string, object> Data { get; }
}