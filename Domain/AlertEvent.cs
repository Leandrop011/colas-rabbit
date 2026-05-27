namespace PublisherEventAlert.Domain;

public class AlertEvent
{
    public string Message { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }

    public AlertEvent() { }

    public AlertEvent(string message, string severity)
    {
        Message = message;
        Severity = severity;
        Timestamp = DateTime.UtcNow;
    }
}
