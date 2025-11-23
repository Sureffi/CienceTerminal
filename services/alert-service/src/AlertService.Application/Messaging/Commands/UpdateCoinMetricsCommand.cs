using CienceTerminal.Contracts.Events;
using MediatR;

namespace AlertService.Application.Messaging.Commands;

/// <summary>
/// Command to update coin metrics in local database and refresh active alerts.
/// Triggered by TokenMetricsUpdatedEvent from Token Metrics Service.
/// </summary>
public record UpdateCoinMetricsCommand(TokenMetricsUpdatedEvent Event) : IRequest;
