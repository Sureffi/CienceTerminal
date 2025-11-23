using TwitterScanner.Domain.Groq;

namespace TwitterScanner.Application.Interfaces;

public interface IGroqClient
{
    Task<GroqResponse?> GenerateAsync(GroqPrompt prompt);
}
