using EventBus.Events;

namespace EventBus.Abstraction;

public interface IRabbitMqEventHandler<in TPRabbitMqEvents> where TPRabbitMqEvents : RabbitMqEvents 
{
    ValueTask<bool> HandleAsync(TPRabbitMqEvents parameters);

}


