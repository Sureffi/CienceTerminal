using CienceTerminal.Contracts.Events;
using MediatR;

namespace TokenMetrics.Application.Messaging.Commands;

/// <summary>
/// Command to process CA mention detected event.
/// Triggered when Twitter Scanner publishes CaMentionDetectedEvent.
/// Responsible for creating Coin records and inserting CaMentionRecords.
/// </summary>
public class ProcessCaMentionDetectedCommand : IRequest
{
    public CaMentionDetectedEvent Event { get; }

    public ProcessCaMentionDetectedCommand(CaMentionDetectedEvent @event)
    {
        Event = @event;
    }
}
