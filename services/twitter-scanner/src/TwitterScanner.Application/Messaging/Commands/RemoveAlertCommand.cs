using CienceTerminal.Contracts.Events;
using MediatR;

namespace TwitterScanner.Application.Messaging.Commands;

/// <summary>
/// Publish alert removal SNS notification
/// </summary>
/// <param name="AlertRemovalEvent"></param>
public record RemoveAlertCommand(AlertRemovalEvent AlertRemovalEvent) : IRequest;
