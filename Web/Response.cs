using Newtonsoft.Json;

namespace Web;

public class Response
{
    private Response(string status, object data)
    {
        Status = status;
        Data = data;
    }
    
    public static Response Success(object data) => new("Success.", data);
    
    public static Response Fail(object data) => new("Fail.", data);

    public string ToJson() => JsonConvert.SerializeObject(this);
    
    [JsonProperty(PropertyName = "status")]
    public string Status { get; }

    [JsonProperty(PropertyName = "data")]
    public object Data { get; }
}