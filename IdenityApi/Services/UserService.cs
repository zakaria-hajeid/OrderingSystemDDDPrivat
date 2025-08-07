using IdenityApi.Models;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Service.Common.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
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
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IConfiguration _config;


        public UserService(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration config, RoleManager<IdentityRole> roleManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _roleManager = roleManager;
        }
        public IEnumerable<Claim>? GetCurrentUser()
        {
            return _httpContextAccessor.HttpContext?.User.Claims;
        }
        public async Task<ClaimsPrincipal> GetClaimToken(string accessToken)
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
            SecurityToken token;
            var result = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out token);
            var jwtSecurityToken = token as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return await Task.FromResult(result);
        }

        public async Task<ResponseModel<string>> CreateaUser(ApplicationUser user, string password, List<string> userRols)

        {
            string guid = Guid.NewGuid().ToString();
            var response = new ResponseModel<string>(guid);
            // إنشاء المستخدم
            List<string> zz = new List<string>();
            var x = JsonConvert.SerializeObject(zz);

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errorsJson = JsonConvert.SerializeObject(result.Errors.Select(e => new { e.Code, e.Description }));
                response.Failed(ErrorResponse.General.SpecificMessage(errorsJson));
                return response;
            }


            // التأكد من أن كل Role موجودة، إذا لا، يتم إنشاؤها
            foreach (var role in userRols.Distinct())
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    // todo:ضيف الرولز بسكريب ثابت بالداتبيز
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole(role));
                    if (!roleResult.Succeeded)
                    {
                        // حذف المستخدم إذا فشل إنشاء الرول لتفادي حالات غير متسقة
                        await _userManager.DeleteAsync(user);
                        response.Failed(ErrorResponse.General.SpecificMessage($"Failed to create role '{role}'"));
                        return response;


                    }
                }
            }

            // ربط المستخدم بالرولز
            var addRoleResult = await _userManager.AddToRolesAsync(user, userRols);
            if (!addRoleResult.Succeeded)
            {
                // حذف المستخدم إذا فشل الربط
                await _userManager.DeleteAsync(user);
                var errorsJson = JsonConvert.SerializeObject(addRoleResult.Errors.Select(e => new { e.Code, e.Description }));
                response.Failed(ErrorResponse.General.SpecificMessage(errorsJson));
                return response;
            }
            response.Succeeded("Succes");
            return response;
        }

        public async Task<IdentityResult> AddUserRole(ApplicationUser userId, string RoleName)
        {
            return await _userManager.AddToRoleAsync(userId, RoleName);

        }
        public async Task<ResponseModel<string>> SignIn(string userName, string password)
        {
            string guid = Guid.NewGuid().ToString();
            var response = new ResponseModel<string>(guid);

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
                response.Succeeded(tokens);
                return response;
            }
            response.Failed(ErrorResponse.General.InvalidPermissions);
            return response;


        }

        public async Task<bool> ValidateAccessPath(string role)
        {

            var userRoleNames = _httpContextAccessor.HttpContext?.User.Claims
                .Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
                .Select(c => c.Value)
                .ToList();

            if (!userRoleNames.Any())
                return false;

            // تحقق هل أحد رولز المستخدم مسموح بها لهذا الباث
            if (!userRoleNames.Contains(role))
                return false;

            return true;
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
