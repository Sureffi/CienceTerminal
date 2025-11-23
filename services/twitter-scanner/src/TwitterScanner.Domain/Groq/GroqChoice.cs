using System.Text.Json.Serialization;

namespace TwitterScanner.Domain.Groq;

public class GroqChoice
{
    [JsonPropertyName("message")]
    public GroqMessage Message { get; set; }

    [JsonPropertyName("finish_reason")]
    public string FinishReason { get; set; }
}
