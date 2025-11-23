using System.Text.Json;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using CienceTerminal.AWS.Abstractions;
using Microsoft.Extensions.Logging;

namespace CienceTerminal.AWS.Services;

public class SnsEventProducer : IEventProducer
{
    private readonly IAmazonSimpleNotificationService _sns;
    private readonly ILogger<SnsEventProducer> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public SnsEventProducer(IAmazonSimpleNotificationService sns, ILogger<SnsEventProducer> logger)
    {
        _sns = sns;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public async Task PublishAsync<T>(string topicArn, T @event, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var message = JsonSerializer.Serialize(@event, _jsonOptions);
            var messageId = GenerateMessageId(@event);

            var request = new PublishRequest
            {
                TopicArn = topicArn,
                Message = message,
                MessageAttributes = new Dictionary<string, MessageAttributeValue>
                {
                    ["event-type"] = new MessageAttributeValue
                    {
                        DataType = "String",
                        StringValue = typeof(T).Name
                    },
                    ["message-id"] = new MessageAttributeValue
                    {
                        DataType = "String",
                        StringValue = messageId
                    }
                }
            };

            var response = await _sns.PublishAsync(request, cancellationToken);

            _logger.LogDebug("Published event {EventType} to topic {TopicArn} with MessageId {MessageId}",
                typeof(T).Name, topicArn, response.MessageId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish event {EventType} to topic {TopicArn}",
                typeof(T).Name, topicArn);
            throw;
        }
    }

    private static string GenerateMessageId<T>(T @event) where T : class
    {
        var alertIdProperty = typeof(T).GetProperty("AlertId");
        if (alertIdProperty?.GetValue(@event) is Guid alertId)
        {
            return alertId.ToString();
        }

        return Guid.NewGuid().ToString();
    }
}