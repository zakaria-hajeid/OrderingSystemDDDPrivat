
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Service.Common;
using System.Text;
using System.Threading.RateLimiting;
using Web.Bff.shopping.HttpAggregator.Services;

internal static class Extensions
{
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

    public static IServiceCollection AddRateLinitingIpAddress(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = 429;
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {

                return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString(),
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 2,
                            Window = TimeSpan.FromSeconds(10)
                        })!;
            });
        });

        return services;
    }

    public static IServiceCollection AddGrpcServices(this IServiceCollection services)
    {

        return services;
    }

    public static IServiceCollection AddAuthinticationOption(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(Options =>
{
    Options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["Jwt:SecretKey"])),
        ValidateIssuer = true,
        ValidateLifetime = true,
        ValidIssuer = configuration["Jwt:Issuer"], //source to validate my token 
        ValidAudience = configuration["Jwt:Audience"], // me 
    };
});

        return services;
    }
    public static IServiceCollection AddHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<UserService>(x=>x.BaseAddress=new Uri( configuration["Identity:apiUrl"]!))
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>() ;

        return services;
    }

}
