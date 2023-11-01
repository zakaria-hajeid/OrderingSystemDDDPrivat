using Service.Common.Extinsions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.AddServiceDefaults();

////Move to common service  by send flag to type of HC
//Helth check for urls 

builder.Services.AddUrlGroupHealthChecks(builder.Configuration);

//Move to common service 
builder.Services.AddRateLinitingIpAddress();


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
