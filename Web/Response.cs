using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Web;

public static class ResponseCreator
{
    public static Response<TData> Create<TData>(TData data)
    {
        return new Response<TData>(data, 200);
    }
    
    public static Response<Error400> Create(Error400 error)
    {
        return new Response<Error400>(error, 400);
    }
    
    public static Response<Error500> Create(Error500 error)
    {
        return new Response<Error500>(error, 500);
    }
}

public class Response<TData> : ContentResult
{
    public Response(TData data, int statusCode)
    {
        Data = data;
        StatusCode = statusCode;

        Content = JsonConvert.SerializeObject(data);
        ContentType = "application/json; charset=utf-8";
    }

    public TData Data { get; }
}

public class Error400
{
    public required string Message { get; init; }
}

public class Error500
{
    public string Message { get; } = "Please report the issue to technical support and attach this response body to your message.";
    
    public required string RequestName { get; init; }
    
    public required string RequestGuid { get; init; }
    
    public required string TraceId { get; init; }
}