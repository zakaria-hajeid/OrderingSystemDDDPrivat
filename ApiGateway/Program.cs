using ApiGateway.Transform;
using Microsoft.Extensions.Configuration;
using Service.Common;
using Service.Common.Extinsions;
using Yarp.ReverseProxy.Transforms;
var builder = WebApplication.CreateBuilder(args);

// Add yarp to the container.
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetRequiredSection("ReverseProxy")).AddTransforms(transformBuilderContext =>
{
    // Add custom transform to specific routes 
    //TODO:put all rout in app setting
    if (transformBuilderContext.Route.RouteId == "Ordering-route")
    {
        transformBuilderContext.AddRequestTransform(async transformContext =>
        {
            var transform = transformContext.HttpContext.RequestServices
                .GetRequiredService<SecurityApiTransform>();
            await transform.ApplyAsync(transformContext);
             
        });
    }
});

//Add http cliebt policy if needed to each request in yarp configration 

/*builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .ConfigureHttpClient((context, client) =>
    {
        // ÊåíÆÉ http client ÇáÎÇÕ ÈÜ YARP
    })
    */


// Authintication api
builder.Services.AddHttpClient("SecurityApi", client =>
{
    client.BaseAddress = new Uri("http://localhost/IdenityApi/api/");
});

builder.Services.AddScoped<SecurityApiTransform>();

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
 * TODO:
5- load balancing 
 */
app.Run();

