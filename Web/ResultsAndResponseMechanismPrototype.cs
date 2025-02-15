/*namespace Web;

public interface IContentData
{
    public string GetData();
}

public interface IContentGenerator
{
    public ContentResult CreateContentResult(IContentData contentData, string contentType = "application/json; charset=utf-8", int statusCode = 200);
}

public class CommandResult
{
    public CommandResult(Guid requestGuid, string requestName)
    {
        RequestGuid = requestGuid;
        RequestName = requestName;
    }

    public Guid RequestGuid { get; set; }
    public string RequestName { get; set; }
}

public class ContentData500Error:IContentData
{
    public ContentData500Error(CommandResult result, HttpContext httpContext)
    {
        Message = "Please report the issue to technical support and attach this response body to your message";
        RequestGuid = result.RequestGuid;
        RequestName = result.RequestName;
        Url = httpContext.Request.GetEncodedUrl();
        TraceId = httpContext.TraceIdentifier;
    }

    public string GetData() => JsonConvert.SerializeObject(this);
    public string Message { get; set; }
    public Guid RequestGuid { get; set; }
    public string RequestName { get; set; }
    public string Url { get; set; }
    public string TraceId { get; set; }
}

public class ContentGenerator: IContentGenerator
{
    public ContentResult CreateContentResult(IContentData contentData, string contentType="application/json; charset=utf-8", int statusCode=200) =>
        new()
        {
            ContentType = contentType, 
            StatusCode = statusCode, 
            Content = contentData.GetData()
        };
}

public class HostStartup : IStartup
{
    public HostStartup(IUnityContainerExecutor executor)
    {
        Executor = executor;
    }

    public async Task Execute(string[] args)
    {
        var commandResult = new CommandResult(Guid.NewGuid(), "TestRequest");
        var contentData = new ContentData500Error(commandResult, new DefaultHttpContext());
        
        var contentGenerator = Executor.Resolve<IContentGenerator>();
        var result = contentGenerator.CreateContentResult(contentData, statusCode: 500);
    }

    IUnityContainerExecutor Executor { get; }
}*/