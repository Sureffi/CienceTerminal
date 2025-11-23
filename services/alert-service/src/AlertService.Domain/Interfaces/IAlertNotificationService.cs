using AlertService.Domain.Entities;

namespace AlertService.Domain.Interfaces;

public interface IAlertNotificationService
{
    Task NotifyAlertAddedAsync(Alert alert);
    Task NotifyAlertRemovedAsync(Guid alertId);
}