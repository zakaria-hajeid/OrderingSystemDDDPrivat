using IdenityApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
namespace IdenityApi.Services
{
    public sealed class UserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _config;


        public UserService(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration config)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
        }
        public IEnumerable<Claim>? GetCurrentUser()
        {
            return _httpContextAccessor.HttpContext?.User.Claims;
        }
        public async Task<ClaimsIdentity> GetClaimToken(string accessToken)
        {

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["Jwt:SecretKey"])),
                ValidateIssuer = false,
                ValidateLifetime = false,
                ValidIssuer = _config["Jwt:Issuer"],
                //ValidAudience = _config["Jwt:Audience"],
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var result =  await tokenHandler.ValidateTokenAsync(accessToken, tokenValidationParameters);
            var jwtSecurityToken = result.SecurityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return result.ClaimsIdentity;
        }

        public async Task<IdentityResult> CreateaUser(ApplicationUser user, string password)
        {
            return await _userManager.CreateAsync(user, password);

        }
        public async Task<IdentityResult> AddUserRole(ApplicationUser userId, string RoleName)
        {
            return await _userManager.AddToRoleAsync(userId, RoleName);

        }
        public async Task<string> SignIn(string userName, string password)
        {
            ApplicationUser? user = await _userManager.FindByNameAsync(userName);
            SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (result.Succeeded)
            {
                ApplicationUser? appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == user.UserName.ToUpper());
                IList<string> Roles = await _userManager.GetRolesAsync(appUser);


                List<Claim> claims = new List<Claim>{
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.NameIdentifier,user.UserName)
            };
                foreach (var role in Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddDays(Convert.ToDouble(_config["Jwt:ExpiresDayes"])),
                    SigningCredentials = creds,
                    Issuer = _config["Jwt:Issuer"],
                   // Audience = _config["Jwt:Audience"],
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
              
                string tokens = tokenHandler.WriteToken(token);
                return tokens;
            }
            return string.Empty;


        }

    }
}
