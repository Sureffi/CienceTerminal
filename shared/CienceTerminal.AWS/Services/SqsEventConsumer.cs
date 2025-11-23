using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using CienceTerminal.AWS.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CienceTerminal.AWS.Services;

public abstract class SqsEventConsumer<T> : BackgroundService, IEventConsumer<T> where T : class
{
    private readonly IAmazonSQS _sqs;
    private readonly ILogger<SqsEventConsumer<T>> _logger;
    private readonly string _queueUrl;
    private readonly JsonSerializerOptions _jsonOptions;

    protected SqsEventConsumer(
        IAmazonSQS sqs,
        ILogger<SqsEventConsumer<T>> logger,
        string queueUrl)
    {
        _sqs = sqs;
        _logger = logger;
        _queueUrl = queueUrl;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting SQS consumer for queue: {QueueUrl}", _queueUrl);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var request = new ReceiveMessageRequest
                {
                    QueueUrl = _queueUrl,
                    MaxNumberOfMessages = 10,
                    WaitTimeSeconds = 20, // Long polling
                    MessageAttributeNames = new List<string> { "All" }
                };

                var response = await _sqs.ReceiveMessageAsync(request, stoppingToken);

                if (response?.Messages == null || response.Messages.Count == 0)
                    continue;

                var tasks = response.Messages.Select(ProcessMessageAsync);
                await Task.WhenAll(tasks);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("SQS consumer for queue {QueueUrl} is being cancelled", _queueUrl);
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing messages from queue {QueueUrl}", _queueUrl);
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        _logger.LogInformation("SQS consumer for queue {QueueUrl} stopped", _queueUrl);
    }

    private async Task ProcessMessageAsync(Message message)
    {
        try
        {
            var snsMessage = JsonSerializer.Deserialize<SnsMessage>(message.Body, _jsonOptions);
            if (snsMessage?.Message == null)
            {
                _logger.LogWarning("Received message with invalid SNS format: {MessageId}", message.MessageId);
                await DeleteMessageAsync(message);
                return;
            }

            var eventData = JsonSerializer.Deserialize<T>(snsMessage.Message, _jsonOptions);
            if (eventData == null)
            {
                _logger.LogWarning("Failed to deserialize event data for message: {MessageId}", message.MessageId);
                await DeleteMessageAsync(message);
                return;
            }

            await HandleEventAsync(eventData, CancellationToken.None);
            await DeleteMessageAsync(message);

            _logger.LogDebug("Successfully processed message {MessageId} for event type {EventType}",
                message.MessageId, typeof(T).Name);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize message {MessageId}. Moving to DLQ.", message.MessageId);
            await DeleteMessageAsync(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message {MessageId}", message.MessageId);
            // Don't delete the message - let it be retried or sent to DLQ
        }
    }

    private async Task DeleteMessageAsync(Message message)
    {
        try
        {
            await _sqs.DeleteMessageAsync(_queueUrl, message.ReceiptHandle);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete message {MessageId}", message.MessageId);
        }
    }

    public abstract Task HandleEventAsync(T @event, CancellationToken cancellationToken = default);

    private class SnsMessage
    {
        public string? Message { get; set; }
    }
}