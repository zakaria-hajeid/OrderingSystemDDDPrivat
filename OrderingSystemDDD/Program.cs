using EventBus.Abstraction;
using EventBus.Events;
using HealthChecks.UI.Client;
using IntegrationEventLogEF.DbContexts;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Ordering.Persistence;
using Ordering.Persistence.SeedWorks;
using OrderingSystemDDD.Configration;
using OrderingSystemDDD.Prsentions;
using Service.Common.Extinsions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.InstallServices(builder.Configuration,
                                 typeof(ApplicationServiceInstalle).Assembly,
                                 typeof(InftastructiorServiceInstaller).Assembly);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.AddServiceDefaults();

var app = builder.Build();
app.AddOrderEndPoints();

using (var scope = app.Services.CreateScope())
{
   /* var bus = scope.ServiceProvider.GetRequiredService<IEventBus>();
    await bus.Publish(new IntegrationEvent() { assymblyName="za"});
   */
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = app.Services.GetService<ILogger<OrderingContextSeed>>();

    await context.Database.MigrateAsync();
    await new OrderingContextSeed().SeedAsync(context, logger);
    var integEventContext = scope.ServiceProvider.GetRequiredService<IntegrationEventLogContext>();
    await integEventContext.Database.MigrateAsync();
}

// Configure the HTTP request pip  eline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapHealthChecks("/hc", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse //to write reportUi helth check
});


app.MapControllers();
app.UseServiceDefaults();
app.Run();
