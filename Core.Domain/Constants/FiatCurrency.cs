using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Core.Domain.Constants;

[JsonConverter(typeof(StringEnumConverter))]
public enum FiatCurrency
{
    Ruble
}