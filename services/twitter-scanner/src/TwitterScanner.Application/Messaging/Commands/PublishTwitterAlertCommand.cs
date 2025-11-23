using CienceTerminal.Contracts.Events;
using MediatR;

namespace TwitterScanner.Application.Messaging.Commands;

/// <summary>
/// Publish a twitter alert SNS notification
/// </summary>
/// <param name="AlertEvent"></param>
public record PublishTwitterAlertCommand(TwitterAlertEvent AlertEvent) : IRequest;
