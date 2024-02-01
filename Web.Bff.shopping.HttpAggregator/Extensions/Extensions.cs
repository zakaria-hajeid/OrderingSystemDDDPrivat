
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Service.Common;
using System.Text;
using Web.Bff.shopping.HttpAggregator.Services;

internal static class Extensions
{
    //make generic and move to common 

    public static IServiceCollection AddReverseProxy(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddReverseProxy().LoadFromConfig(configuration.GetRequiredSection("ReverseProxy"));

        return services;
    }

    public static IServiceCollection AddUrlGroupHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddUrlGroup(_ => new Uri(configuration.GetRequiredValue("OrderingUrlHC")), name: "Ordering-check", tags: new string[] { "catalogapi" })
            .AddUrlGroup(_ => new Uri(configuration.GetRequiredValue("SignalRUrlHC")), name: "SignalR-check", tags: new string[] { "orderingapi" })
            .AddUrlGroup(_ => new Uri(configuration.GetRequiredValue("WebSocketUrlHC")), name: "WebSocket-check", tags: new string[] { "basketapi" })
            .AddUrlGroup(_ => new Uri(configuration.GetRequiredValue("IdentityUrlHC")), name: "Identity-check", tags: new string[] { "identityapi" });
        return services;
    }


    public static IServiceCollection AddGrpcServices(this IServiceCollection services)
    {

        return services;
    }

    //make generic and move to common 

    public static IServiceCollection AddHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<UserService>(x => x.BaseAddress = new Uri(configuration["Identity:apiUrl"]!))
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>();

        return services;
    }

}
