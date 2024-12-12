using FluentResults;

namespace Core.Application;

public class CommandResult : ResultBase
{
    public CommandResult(object value)
    {
        _value = value;
    }

    public CommandResult()
    {
        _value = default;
    }

    public object Value => _value ?? throw new Exception("Value is empty.");

    private readonly object? _value;
    
    public Guid RequestGuid { get; set; }

    public string RequestName { get; set; } = string.Empty;
}