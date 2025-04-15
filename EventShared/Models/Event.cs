namespace EventShared.Models;

public enum EventType
{
    Type1 = 1,
    Type2 = 2,
    Type3 = 3
}

public class Event
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public EventType Type { get; set; }
    public DateTime Time { get; set; } = DateTime.UtcNow;

    public Guid? IncidentId { get; set; }
    public Incident? Incident { get; set; }
}