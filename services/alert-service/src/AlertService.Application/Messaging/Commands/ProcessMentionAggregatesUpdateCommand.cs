using CienceTerminal.Contracts.Events;
using MediatR;

namespace AlertService.Application.Messaging.Commands;

/// <summary>
/// Command to process mention aggregates update event.
/// Triggered when Token Metrics Service publishes MentionAggregatesUpdatedEvent.
/// </summary>
public class ProcessMentionAggregatesUpdateCommand : IRequest
{
    public MentionAggregatesUpdatedEvent Event { get; }

    public ProcessMentionAggregatesUpdateCommand(MentionAggregatesUpdatedEvent @event)
    {
        Event = @event;
    }
}
