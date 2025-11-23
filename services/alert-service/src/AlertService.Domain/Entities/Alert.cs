using CienceTerminal.Contracts.Enums;

namespace AlertService.Domain.Entities;

public abstract class Alert
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public AlertSeverity Severity { get; set; }
    public abstract AlertType Type { get; set; }
    public abstract string Title { get; }
    public abstract string Message { get; }
}
