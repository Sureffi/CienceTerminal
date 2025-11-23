using System.Text.Json.Serialization;

namespace TwitterScanner.Domain.Groq;

public class GroqResponse
{
    [JsonPropertyName("choices")]
    public GroqChoice[] Choices { get; set; }

    [JsonPropertyName("usage")]
    public GroqUsage Usage { get; set; }
}
