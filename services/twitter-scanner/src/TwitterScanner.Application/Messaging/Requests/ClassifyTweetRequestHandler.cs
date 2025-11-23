using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using TwitterScanner.Application.Interfaces;
using TwitterScanner.Domain.Groq;
using TwitterScanner.Domain.Groq.Prompts;

namespace TwitterScanner.Application.Messaging.Requests;

public class ClassifyTweetRequestHandler : IRequestHandler<ClassifyTweetRequest, TweetClassifierResult>
{
    private readonly IGroqClient _groqClient;
    private readonly ILogger<ClassifyTweetRequestHandler> _logger;

    public ClassifyTweetRequestHandler(IGroqClient groqClient, ILogger<ClassifyTweetRequestHandler> logger)
    {
        _groqClient = groqClient;
        _logger = logger;
    }

    async Task<TweetClassifierResult> IRequestHandler<ClassifyTweetRequest, TweetClassifierResult>.Handle(ClassifyTweetRequest request, CancellationToken cancellationToken)
    {
        // Use TweetClassifier prompt on GroqCLient
        // Return TweetClassifier result
        var prompt = new TweetClassifierPrompt(request.Tweet.Content);
        GroqResponse? groqResponse = await _groqClient.GenerateAsync(prompt);


        // Convert GroqResponse to TweetClassifierResult
        if (groqResponse?.Choices?.Length > 0 && groqResponse.Choices[0]?.Message?.Content != null)
        {

            var jsonContent = groqResponse.Choices[0].Message.Content.Trim();

            if (jsonContent.StartsWith("```"))
            {
                // Remove opening ```json or ```
                var firstNewline = jsonContent.IndexOf('\n');
                if (firstNewline > 0)
                {
                    jsonContent = jsonContent.Substring(firstNewline + 1);
                }

                // Remove closing ```
                var lastBackticks = jsonContent.LastIndexOf("```");
                if (lastBackticks > 0)
                {
                    jsonContent = jsonContent.Substring(0, lastBackticks);
                }

                jsonContent = jsonContent.Trim();
            }

            try
            {
                // TODO: Fix null reference?
                // Return deserialized object if succesfully deserialized
                return JsonSerializer.Deserialize<TweetClassifierResult>(jsonContent)!;
            }
            catch (JsonException ex)
            {
                // Failed to deserialize object, classify as spam
                _logger.LogDebug(ex, "Failed to parse ai response for tweet classification. Original: {Original}, Clened: {jsonContent}", groqResponse.Choices[0].Message.Content, jsonContent);

                return new TweetClassifierResult
                {
                    Classification = "SPAM",
                    Confidence = 0.0,
                    Reasoning = "Failed to parse AI response"
                };
            }
        }

        // If not returned at this point, response from groq was invalid
        // TODO: Better handling for this case
        _logger.LogWarning("Reached fallback for ai classifier");
        return new TweetClassifierResult
        {
            Classification = "SPAM",
            Confidence = 0.0,
            Reasoning = "No response from Groq API"
        };
    }
}
