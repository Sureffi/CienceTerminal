using CienceTerminal.Contracts.Events;
using MediatR;

namespace AlertService.Application.Messaging.Commands;

public record ProcessTwitterAlertCommand(TwitterAlertEvent AlertEvent) : IRequest;