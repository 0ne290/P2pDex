using System.Dynamic;
using FluentResults;

namespace Core.Application;

public class CommandResult : ResultBase
{
    public CommandResult(ExpandoObject value)
    {
        _value = value;
    }

    public CommandResult()
    {
        _value = default;
    }

    public ExpandoObject Value => _value ?? throw new Exception("Value is empty.");

    private readonly ExpandoObject? _value;
    
    public Guid RequestGuid { get; set; }

    public string RequestName { get; set; } = string.Empty;
}