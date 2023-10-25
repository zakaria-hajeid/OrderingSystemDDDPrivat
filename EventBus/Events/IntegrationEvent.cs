using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Serialization;

namespace EventBus.Events;

public record IntegrationEvent
{
    public IntegrationEvent()
    {
        Id = Guid.NewGuid();
        CreationDate = DateTime.UtcNow;
    }
    public IntegrationEvent(Guid id, DateTime createDate)
    {
        Id = id;
        CreationDate = createDate;
    }
    public string  eventContent { get; set; }
    public Guid Id { get; private init; }

    public DateTime CreationDate { get; private init; }

    private string _assymblyName;

    public string assymblyName { get => _assymblyName; set => _assymblyName = value; }
    
}


