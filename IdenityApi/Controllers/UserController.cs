using Azure.Core;
using IdenityApi.Model;
using IdenityApi.Models;
using IdenityApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Service.Common.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IdenityApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserModel createUserModel)
        {
            try
            {
                ApplicationUser applicationUser = new ApplicationUser()
                {
                    UserName = createUserModel.Name,

                };
                var result = await _userService.CreateaUser(applicationUser, createUserModel.password, createUserModel.userRols);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("LoginUser")]
        public async Task<IActionResult> LoginUser([FromBody] LoginUserModel loginUserModel)
        {
            try
            {
                var result = await _userService.SignIn(loginUserModel.userName, loginUserModel.Password);
                if (!result.IsSuccess)
                {
                    return BadRequest();
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("GetCuentClaim")]
        public async Task<IActionResult> GetCuentClaim()
        {

            return Ok(_userService.GetCurrentUser().ToList().SelectMany(x => x.Value).ToList()); //edit response 
        }
        [Authorize]
        [HttpGet("GetClaimFromToken")]
        public async Task<IActionResult> GetClaimFromToken(string accessToken)
        {

            return Ok(await _userService.GetClaimToken(accessToken)); //edit response 
        }
        [Authorize]
        [HttpGet("IsauthinticatedUser")]
        public async Task<IActionResult> IsauthinticatedUser()
        {

            var authHeader = Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return Unauthorized("Missing or invalid Authorization header");


            var token = authHeader.Substring("Bearer ".Length).Trim();
            var result = await _userService.GetClaimToken(token);
                if (result is not null)
            {
                return Ok();


            }
            return Unauthorized();
        }

        [HttpGet("TEst")]
        public async Task<IActionResult> TEst()
        {
            return Ok();
        }

    }
}
