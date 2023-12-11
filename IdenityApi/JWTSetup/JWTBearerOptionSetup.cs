using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace IdenityApi.JWTSetup
{
    public class JWTBearerOptionSetup : IConfigureOptions<JwtBearerOptions>
    {
        private readonly IConfiguration _config;

        public JWTBearerOptionSetup(IConfiguration config)
        {
            _config = config;
        }

        public void Configure(JwtBearerOptions jwtBearerOptions)
        {
            jwtBearerOptions.TokenValidationParameters = new()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["Jwt:SecretKey"])),
                ValidateIssuer = false,
                ValidateLifetime = false,
                ValidIssuer = _config["Jwt:Issuer"],
                ValidAudience = _config["Jwt:Audience"],
            };

        }
    }
}
