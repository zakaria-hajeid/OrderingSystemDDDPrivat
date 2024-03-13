using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Serialization;

namespace EventBus.Events;

public abstract record IntegrationEvent
{
    public IntegrationEvent()
    {
        Id = Guid.NewGuid();
        CreationDate = DateTime.UtcNow;
        setEventType();
    }
    public IntegrationEvent(Guid id, DateTime createDate)
    {
        Id = id;
        CreationDate = createDate;
    }
    public Guid Id { get;  init; }

    public DateTime CreationDate { get;  init; }
    protected int EventType { get; set; }

    public int eventType => EventType;
    public abstract void setEventType();
}


