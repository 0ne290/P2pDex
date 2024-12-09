using Newtonsoft.Json;

namespace Web;

public class Response
{
    private Response(string status) => Status = status;
    
    private Response(string status, params (string Key, object Value)[] data)
    {
        Status = status;

        foreach (var keyValuePair in data)
            Data.Add(keyValuePair.Key, keyValuePair.Value);
    }

    public static Response Success() => new("Success.");
    
    public static Response Success(params (string Key, object Value)[] data) => new("Success.", data);
    
    public static Response Fail() => new("Fail.");
    
    public static Response Fail(params (string Key, object Value)[] data) => new("Fail.", data);

    public string ToJson() => JsonConvert.SerializeObject(this);
    
    [JsonProperty(PropertyName = "status")]
    public string Status { get; }

    [JsonProperty(PropertyName = "data")]
    public Dictionary<string, object> Data { get; } = new();
}