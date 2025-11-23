namespace CienceTerminal.Contracts.Events;

public class AlertRemovalEvent
{
    public Guid AlertId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
}
