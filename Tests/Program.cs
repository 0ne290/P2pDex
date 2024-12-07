﻿using Newtonsoft.Json;

namespace Tests;

public class MyModel
{
    public required Guid Value { get; init; }
}

internal class Program
{
    private static void Main()
    {
        var c = $"{{ \"Value\": \"{Guid.NewGuid().ToString()}\" }}";

        var g = JsonConvert.DeserializeObject<MyModel>(c);
        
        Console.WriteLine(g?.Value.ToString() ?? "TyT 6bIjo He4eH3ypHoe cjoBo.");
    }
}