using Newtonsoft.Json;

namespace CoreConsoleTests;

public class TestModel
{
    [JsonProperty(PropertyName = "name")]
    public required string Name { get; init; }
    
    [JsonIgnore]
    [JsonProperty(PropertyName = "password")]
    public required string Password { get; init; }
}