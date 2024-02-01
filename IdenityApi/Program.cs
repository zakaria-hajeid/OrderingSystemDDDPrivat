using IdenityApi.Data;
using IdenityApi.JWTSetup;
using IdenityApi.Models;
using IdenityApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<UserService>();
builder.Services.AddCors();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityDB"), x=>x.MigrationsAssembly("IdenityApi"));
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(Options =>
{
    Options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:SecretKey"]!)),
        ValidateIssuer = true,
        ValidateLifetime = true,
        ValidateAudience = false,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
    };
});
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

//builder.Services.ConfigureOptions<JWTBearerOptionSetup>(); Alternative way try it 

IdentityBuilder Idbuilder = builder.Services.AddIdentityCore<ApplicationUser>(opt =>
{
    opt.Password.RequireDigit = false;
    opt.Password.RequiredLength = 4;
    opt.Password.RequireNonAlphanumeric = false;//@ or space etc...
    opt.Password.RequireUppercase = false;
});

//Idbuilder = new IdentityBuilder(Idbuilder.UserType, typeof(IdentityRole<int>), Idbuilder.Services);
Idbuilder.AddEntityFrameworkStores<ApplicationDbContext>();
//Idbuilder.AddRoleValidator<RoleValidator<IdentityRole<int>>>();
//Idbuilder.AddRoleManager<RoleManager<IdentityRole<int>>>();
Idbuilder.AddSignInManager<SignInManager<ApplicationUser>>();

builder.Services.AddIdentityCore<ApplicationUser>(o =>
{
    o.Stores.MaxLengthForKeys = 128;
    o.SignIn.RequireConfirmedAccount = true;
}).AddDefaultTokenProviders();


/*builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();
*/
builder.Services.Configure<IdentityOptions>(options =>
{

    options.SignIn.RequireConfirmedAccount = false;


});

//add using configure option inteface

/*.AddJwtBearer(Options =>
    {
        Options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["Jwt:SecretKey"])),
            ValidateIssuer = false,
            ValidateLifetime = false,
            ValidIssuer = Configuration["Jwt:Issuer"],
            ValidAudience = Configuration["Jwt:Audience"],
        };
    });*/

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
