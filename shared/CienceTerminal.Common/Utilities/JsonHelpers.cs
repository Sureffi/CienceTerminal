using System.Text.Json;

namespace CienceTerminal.Common.Utilities;

public static class JsonHelpers
{
    private static readonly JsonSerializerOptions _standardOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public static JsonSerializerOptions GetStandardOptions() => _standardOptions;

    public static string Serialize<T>(T obj) => JsonSerializer.Serialize(obj, _standardOptions);

    public static T? Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, _standardOptions);
}