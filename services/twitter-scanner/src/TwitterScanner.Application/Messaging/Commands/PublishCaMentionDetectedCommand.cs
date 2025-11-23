using CienceTerminal.Contracts.Events;
using MediatR;

namespace TwitterScanner.Application.Messaging.Commands;

/// <summary>
/// Publish CaMentionDetected SNS notification
/// </summary>
/// <param name="Event"></param>
public record PublishCaMentionDetectedCommand(CaMentionDetectedEvent Event) : IRequest;
