using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Core.Domain.Constants;

[JsonConverter(typeof(StringEnumConverter))]
public enum Cryptocurrency
{
    Ethereum
}