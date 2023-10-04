using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Ordering.Persistence;
using OrderingSystemDDD.Configration;
using OrderingSystemDDD.Prsentions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.InstallServices(builder.Configuration,
                                 typeof(ApplicationServiceInstalle).Assembly,
                                 typeof(InftastructiorServiceInstaller).Assembly);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.AddOrderEndPoints();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
  
    await context.Database.MigrateAsync();
    
}

// Configure the HTTP request pip  eline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
