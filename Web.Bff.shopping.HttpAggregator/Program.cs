using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Service.Common.Extinsions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.AddServiceDefaults();
//Helth check for urls 

builder.Services.AddHealthChecks()
.AddUrlGroup(_ => new Uri(builder.Configuration.GetRequiredValue("OrderingUrlHC")), name: "Ordering-check", tags: new string[] { "catalogapi" })
   .AddUrlGroup(_ => new Uri(builder.Configuration.GetRequiredValue("SignalRUrlHC")), name: "SignalR-check", tags: new string[] { "orderingapi" })
   .AddUrlGroup(_ => new Uri(builder.Configuration.GetRequiredValue("WebSocketUrlHC")), name: "WebSocket-check", tags: new string[] { "basketapi" })
   .AddUrlGroup(_ => new Uri(builder.Configuration.GetRequiredValue("IdentityUrlHC")), name: "Identity-check", tags: new string[] { "identityapi" });


builder.Services.AddReverseProxy(builder.Configuration);
builder.Services.AddCors(options =>
{
    // TODO: Read allowed origins from configuration
    options.AddPolicy("CorsPolicy",
        builder => builder
        .SetIsOriginAllowed((host) => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

List<KeyValuePair<string, string>> hcRout = new List<KeyValuePair<string, string>>()
{
    new KeyValuePair<string, string>("OrderingUrlHC","Ordering-check"),
     new KeyValuePair<string, string>("SignalRUrlHC","SignalR-check"),
    new KeyValuePair<string, string>("WebSocketUrlHC","WebSocket-check"),
    new KeyValuePair<string, string>("IdentityUrlHC","Identity-check"),

};

app.MapSpeacificHelthCheck(hcRout);

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.UseServiceDefaults();

app.MapControllers();
app.MapReverseProxy();
app.Run();
