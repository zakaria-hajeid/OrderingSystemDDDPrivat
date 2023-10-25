using EventBus.Abstraction;
using EventBus.Events;
using MassTransit;
using Ordering.SignalrHub.IntegrationEventHandling.EventHandling;
using Service.Common.Extinsions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Refactor to use method to prepare this just call  AddSharedServices in common service libraray 
Dictionary<string, List<Action<IRabbitMqReceiveEndpointConfigurator, IBusRegistrationContext>>> queueNameWithConsumers = new Dictionary<string, List<Action<IRabbitMqReceiveEndpointConfigurator, IBusRegistrationContext>>>();
queueNameWithConsumers.Add("IntegrationEvent", new List<Action<IRabbitMqReceiveEndpointConfigurator, IBusRegistrationContext>>()
{
    (endpoint,context) =>
    endpoint.ConfigureConsumer<OrderStatusChangedToSubmittedIntegrationEventHandler>(context)
});

List<Action<IBusRegistrationConfigurator>> Consumers = new List<Action<IBusRegistrationConfigurator>>()
{
    (bus)=>bus.AddConsumer<OrderStatusChangedToSubmittedIntegrationEventHandler>()
};


builder.Services.AddSharedServices(builder.Configuration,true,queueNameWithConsumers, Consumers.ToArray()) ;


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
