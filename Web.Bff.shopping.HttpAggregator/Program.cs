using Asp.Versioning;
using GrpcOrder;
using Microsoft.Extensions.Options;
using Service.Common;
using Service.Common.Extinsions;
using Web.Bff.shopping.HttpAggregator.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.AddServiceDefaults();
////Move to common service  by send flag to type of HC
//Helth check for urls 

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<HttpClientAuthorizationDelegatingHandler>();
builder.Services.AddSwaggerGen();
builder.Services.AddUrlGroupHealthChecks(builder.Configuration);

builder.Services.AddRateLinitingIpAddress();
builder.Services.AddAuthinticationOption(builder.Configuration);

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient(builder.Configuration);

builder.Services.AddReverseProxy(builder.Configuration);
builder.Services.AddScoped<OrderGrpcService>();
builder.Services.AddGrpcClient<OrderRpc.OrderRpcClient>((services, options) =>
{
    options.Address = new Uri("https://localhost:7264/");
});


//coustem  trnasform
/*builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms ( async transform =>
    {
        transform.AddRequestTransform(async(context) =>
        {
            var httpContext = context.HttpContext;
            string? requestBody = "";
            httpContext.Request.Body.Position = 0;
            using (StreamReader sr = new StreamReader(httpContext.Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true))
            {
                requestBody = await sr.ReadToEndAsync();
            }

            var replacedContent = requestBody.Replace("%PLACEHOLDER%", "Hello");
            var requestContent = new StringContent(replacedContent, Encoding.UTF8, "application/json");
            httpContext.Request.Body = requestContent.ReadAsStream();
            httpContext.Request.ContentLength = httpContext.Request.Body.Length;
        });
    })
    ;*/

//Api versioning 
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version"));
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddCors();

builder.Services.AddCors(options =>
{
    // TODO: Read allowed origins from configuration
    options.AddPolicy("CorsPolicy",
        builder => builder
        //.SetIsOriginAllowed((host) => true)
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
        );
});
var app = builder.Build();
// Configure the HTTP request pipeline.
/*if (app.Environment.IsProduction())
{
  
}*/
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
List<KeyValuePair<string, string>> hcRout = new List<KeyValuePair<string, string>>()
{
    new KeyValuePair<string, string>("OrderingUrlHC","Ordering-check"),
     new KeyValuePair<string, string>("SignalRUrlHC","SignalR-check"),
    new KeyValuePair<string, string>("WebSocketUrlHC","WebSocket-check"),
    new KeyValuePair<string, string>("IdentityUrlHC","Identity-check"),

};
app.MapSpeacificHelthCheck(hcRout);
app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.UseServiceDefaults();

app.MapControllers();
app.MapReverseProxy();

//using token bucket Manually by pipline without add rate limiting servive 

/*app.UseRateLimiter(new RateLimiterOptions
{

    GlobalLimiter= PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        if(context.Request.Path== "/GetWeatherForecast")
        {
            return RateLimitPartition.GetNoLimiter<string>("UnlimitRequest");
        }
        return RateLimitPartition.GetTokenBucketLimiter("Token", _ =>
        {

            return new TokenBucketRateLimiterOptions()
            {
                TokenLimit = 10,
                QueueProcessingOrder = QueueProcessingOrder.NewestFirst,
                QueueLimit = 0,
                ReplenishmentPeriod = TimeSpan.FromSeconds(5),
                TokensPerPeriod = 5

            };
        });
    }),RejectionStatusCode=429


});*/

app.UseRateLimiter();
app.Run();
