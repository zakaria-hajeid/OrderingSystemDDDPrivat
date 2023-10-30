using EventBus.Abstraction;
using EventBus.Events;
using IntegrationEventLogEF.DbContexts;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Ordering.SignalrHub;
using Ordering.SignalrHub.Hubs;
using Ordering.SignalrHub.IntegrationEventHandling.EventHandling;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<OrderStatusChangedToSubmittedIntegrationEventHandler>();

Dictionary<string, List<Action<IRabbitMqReceiveEndpointConfigurator, IBusRegistrationContext>>> queueNameWithConsumers = new Dictionary<string, List<Action<IRabbitMqReceiveEndpointConfigurator, IBusRegistrationContext>>>();
queueNameWithConsumers.Add("IntegrationEvent", new List<Action<IRabbitMqReceiveEndpointConfigurator, IBusRegistrationContext>>()
{
    (endpoint,context) =>
    {
        endpoint.Consumer<OrderStatusChangedToSubmittedIntegrationEventHandler>(context);
        endpoint.UseMessageRetry(x=>x.Interval(int.Parse(builder.Configuration["EventBusMessageBroker:RetryCount"]),int.Parse(builder.Configuration["EventBusMessageBroker:Interval"])));
    }
});
builder.Services.AddSharedServices(builder.Configuration,true,queueNameWithConsumers) ;
builder.Services.AddSignalR();
var app = builder.Build();
app.MapHub<NotificationsHub>("/hub/notificationhub");



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
