using EventBus.Events;

namespace EventBus.Abstraction.RabbitMq;

public interface IRabbitMqEventHandler<in TPRabbitMqEvents> where TPRabbitMqEvents : RabbitMqEvents
{
    ValueTask<bool> HandleAsync(TPRabbitMqEvents parameters);

}


