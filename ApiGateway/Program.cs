using Microsoft.Extensions.Configuration;
using Service.Common;
using Service.Common.Extinsions;
var builder = WebApplication.CreateBuilder(args);

// Add yarp to the container.
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetRequiredSection("ReverseProxy"));

// add Authintication 
builder.Services.AddAuthinticationOption(builder.Configuration);
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAuthenticatedUser", policy =>
          policy.RequireAuthenticatedUser());    // Add other policies as needed
});

//Add RateLImitter
builder.Services.AddRateLinitingIpAddress();

//ADD HC :
builder.Services.AddHealthChecks()
          .AddUrlGroup(_ => new Uri(builder.Configuration.GetRequiredValue("OrderingUrlHC")), name: "Ordering-check", tags: new string[] { "catalogapi" })
          .AddUrlGroup(_ => new Uri(builder.Configuration.GetRequiredValue("SignalRUrlHC")), name: "SignalR-check", tags: new string[] { "orderingapi" })
          .AddUrlGroup(_ => new Uri(builder.Configuration.GetRequiredValue("WebSocketUrlHC")), name: "WebSocket-check", tags: new string[] { "basketapi" })
          .AddUrlGroup(_ => new Uri(builder.Configuration.GetRequiredValue("IdentityUrlHC")), name: "Identity-check", tags: new string[] { "identityapi" });


var app = builder.Build();



//TODO: make all it as installtion service in shared place
app.MapReverseProxy();
app.UseServiceDefaults();
app.UseRateLimiter();

List<KeyValuePair<string, string>> hcRout = new List<KeyValuePair<string, string>>()
{
    new KeyValuePair<string, string>("OrderingUrlHC","Ordering-check"),
     new KeyValuePair<string, string>("SignalRUrlHC","SignalR-check"),
    new KeyValuePair<string, string>("WebSocketUrlHC","WebSocket-check"),
    new KeyValuePair<string, string>("IdentityUrlHC","Identity-check"),

};
app.MapSpeacificHelthCheck(hcRout);

/*
4-UseCors bolicy 
5- load balancing 
 */
app.Run();

