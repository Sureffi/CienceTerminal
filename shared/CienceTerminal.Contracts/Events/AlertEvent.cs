using CienceTerminal.Contracts.Enums;

namespace CienceTerminal.Contracts.Events;

/// <summary>
/// Base alert class
/// </summary>
public abstract class AlertEvent
{
    public Guid AlertId { get; set; } = Guid.NewGuid();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public AlertSeverity Severity { get; set; }
    public abstract AlertType Type { get; }
    public string Source { get; set; } = "TwitterScanner"; // TODO: Make abstract?
}
