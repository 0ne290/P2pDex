using FluentResults;

namespace Core.Application;

public class CommandResult : ResultBase
{
    public CommandResult(Dictionary<string, object> value)
    {
        _value = value;
    }

    public CommandResult()
    {
        _value = default;
    }

    public Dictionary<string, object> Value => _value ?? throw new Exception("Value is empty.");

    private readonly Dictionary<string, object>? _value;
    
    public Guid RequestGuid { get; set; }

    public string RequestName { get; set; } = string.Empty;
}