
using IntegrationEventLogEF.Enums;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace IntegrationEventLogEF.Entities;

public class IntegrationEventOutbox
{
    private static readonly JsonSerializerOptions s_indentedOptions = new() { WriteIndented = true };
    private static readonly JsonSerializerOptions s_caseInsensitiveOptions = new() { PropertyNameCaseInsensitive = true };

    public IntegrationEventOutbox() { }
    private IntegrationEventOutbox(IntegrationEvent @event, Guid transactionId)
    {
        EventId = @event.Id;
        CreationTime = @event.CreationDate;
        EventTypeName = @event.GetType().FullName;
        Content = JsonSerializer.Serialize(@event, @event.GetType(), s_indentedOptions);
        State = EventStateEnum.NotPublished;
        TimesSent = 0;
        TransactionId = transactionId.ToString();
    }
    public Guid EventId { get; private set; }
    public string EventTypeName { get; private set; }
    [NotMapped]
    public string EventTypeShortName => EventTypeName.Split('.')?.Last();
    [NotMapped]
    public IntegrationEvent IntegrationEvent { get; private set; }
    public EventStateEnum State { get; set; }
    public int TimesSent { get; set; }
    public DateTime CreationTime { get; private set; }
    public string Content { get; private set; }
    public string TransactionId { get; private set; }

    public IntegrationEventOutbox DeserializeJsonContent(Type type)
    {
        IntegrationEvent = JsonSerializer.Deserialize(Content, type, s_caseInsensitiveOptions) as IntegrationEvent;
        return this;
    }
    public static IntegrationEventOutbox AddingNewEvent(IntegrationEvent @event, Guid transactionId)
    {
        return new IntegrationEventOutbox(@event, transactionId);

    }
    public void UpdateEventStatus(EventStateEnum status)
    {
        if (status == EventStateEnum.InProgress)
            this.TimesSent++;
          this.State = status;

    }

}
