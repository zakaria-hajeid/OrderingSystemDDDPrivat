using MassTransit;
using Microsoft.EntityFrameworkCore;
using Service.Common.Extinsions;
using Webhooks.api.Application;
using Webhooks.api.Application.IntegrationEvents;
using Webhooks.api.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.AddServiceDefaults();
builder.Services.AddDbContext<WebhooksContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetRequiredConnectionString("WebHooksDB"),

        sqlOptions =>
        {
            sqlOptions.MigrationsAssembly("Webhooks.api");
            sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay:
                        TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
        });
});


//sql-helth check 
builder.Services.AddHealthChecks().
AddSqlServer(_ =>
             builder.Configuration.GetRequiredConnectionString("WebHooksDB"),
             name: "WebhooksApiDb-check",
             tags: new string[] { "ready" });

//HttpClient
builder.Services.AddHttpClient("GrantClient")
               .SetHandlerLifetime(TimeSpan.FromMinutes(5));
//Yo can use by inject http client factory and create _httpClientFactory.CreateClient("GrantClient");

builder.Services.AddTransient<IWebhooksRetriever, WebhooksRetriever>();
builder.Services.AddTransient<IWebhooksSender, WebhooksSender>();
//register consumer 
builder.Services.AddScoped<OrderStatusChangedToPaidIntegrationEventHandler>();

Dictionary<string, List<Action<IRabbitMqReceiveEndpointConfigurator, IBusRegistrationContext>>> queueNameWithConsumers = new Dictionary<string, List<Action<IRabbitMqReceiveEndpointConfigurator, IBusRegistrationContext>>>();
queueNameWithConsumers.Add("IntegrationEvent", new List<Action<IRabbitMqReceiveEndpointConfigurator, IBusRegistrationContext>>()
{
    (endpoint,context) =>
    {
        endpoint.Consumer<OrderStatusChangedToPaidIntegrationEventHandler>(context);
        int interval,RetryCount;
        int.TryParse(builder.Configuration["EventBusMessageBroker:Interval"],out  interval);
        int.TryParse(builder.Configuration["EventBusMessageBroker:RetryCount"],out  RetryCount);
        endpoint.UseMessageRetry(x=>x.Interval(RetryCount,interval));
    }
});
builder.Services.AddEventBusSharedServices(builder.Configuration, true, queueNameWithConsumers);


var app = builder.Build();


List<KeyValuePair<string, string>> hcRout = new List<KeyValuePair<string, string>>()
{
    new KeyValuePair<string, string>("SqlCheck","WebhooksApiDb-check"),
};

app.MapSpeacificHelthCheck(hcRout);
//when call this point we will execut the sql quety in master database to i nsure is helthy or not 
//and you can coustem check in any time 

app.UseServiceDefaults();

using (var scope = app.Services.CreateScope())
{
    /* var bus = scope.ServiceProvider.GetRequiredService<IEventBus>();
     await bus.Publish(new IntegrationEvent() { assymblyName="za"});
    */
    var context = scope.ServiceProvider.GetRequiredService<WebhooksContext>();

    await context.Database.MigrateAsync();
    await new WebhooksContextSeedData().SeedAsync(context);
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.MapControllers();

app.Run();
