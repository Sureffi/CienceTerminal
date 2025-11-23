using System.Text.Json.Serialization;

namespace TwitterScanner.Domain.Groq;

public class GroqPrompt
{
    [JsonPropertyName("messages")]
    public GroqMessage[] Messages { get; set; }

    [JsonPropertyName("model")]
    public string Model { get; set; }
}
