using System.Text.Json.Serialization;

namespace TwitterScanner.Domain.Groq;

public class GroqMessage
{
    [JsonPropertyName("role")]
    public required string Role { get; set; }

    [JsonPropertyName("content")]
    public required string Content { get; set; }
}
