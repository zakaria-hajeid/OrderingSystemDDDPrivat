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
        public  async Task<ClaimsPrincipal> GetClaimToken(string accessToken)
        {

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["Jwt:SecretKey"]!)),
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidIssuer = _config["Jwt:Issuer"],
                ValidAudience = _config["Jwt:Audience"],
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token ; 
            var result =   tokenHandler.ValidateToken(accessToken, tokenValidationParameters,out  token );
            var jwtSecurityToken = token as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return await Task.FromResult(result);
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
                //IList<string> Roles = await _userManager.GetRolesAsync(appUser); add role validator in app setting to active 


                List<Claim> claims = new List<Claim>{
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.NameIdentifier,user.UserName)
            };
               

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddDays(Convert.ToDouble(_config["Jwt:ExpiresDayes"])),
                    SigningCredentials = creds,
                    Issuer = _config["Jwt:Issuer"],
                    Audience = _config["Jwt:Audience"],
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
              
                string tokens = tokenHandler.WriteToken(token);
                return tokens;
            }
            return string.Empty;


        }


        //todo :add refresh token 
        /*public async Task<LoginResult> RefreshToken(LoginResultDtos TokenModel)
        {

            string accessToken = TokenModel.Token;
            string refreshToken = TokenModel.RefreshToken;
            var principal = await tokenServices.GetClaimToken(accessToken);
            var username = principal.Identity.Name; //this is mapped to the Name claim by default
            var user = await _Db.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                return null;
            IList<string> Roles = await _userManager.GetRolesAsync(user);

            tokenStrategyContext = new TokenStrategyContext(TokenFactoryMethod(TokenType.JWT));
            string refreshTokenCreate = await tokenServices.CreateRefreshToken();

            object token = await tokenStrategyContext.CreateToken(user, Roles, refreshTokenCreate);

            return new LoginResult()
            {
                Token = (string)token.GetType()?.GetProperty("token")?.GetValue(token),
                RefreshToken = (string)token.GetType()?.GetProperty("refreshToken")?.GetValue(token)

            };
        }*/

    }
}
