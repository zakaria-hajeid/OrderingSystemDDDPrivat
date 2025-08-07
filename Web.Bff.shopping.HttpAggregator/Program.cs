using Asp.Versioning;
using GrpcOrder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Service.Common;
using Service.Common.Extinsions;
using System.Text;
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


builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient(builder.Configuration);

builder.Services.AddScoped<OrderGrpcService>();
builder.Services.AddGrpcClient<OrderRpc.OrderRpcClient>((services, options) =>
{
    options.Address = new Uri("https://localhost:7264/");
});


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


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseServiceDefaults();

app.MapControllers();
app.Run();
