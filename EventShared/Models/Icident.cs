namespace EventShared.Models;

public enum IncidentType
{
    Type1 = 1,
    Type2 = 2,
    Type3 = 3
}

public class Incident
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public IncidentType Type { get; set; }
    public DateTime Time { get; set; } = DateTime.UtcNow;

    public List<Guid> EventIds { get; set; } = new List<Guid>();
    
    public List<Event> Events { get; set; } = new();
}
