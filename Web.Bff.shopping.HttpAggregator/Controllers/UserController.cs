using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Bff.shopping.HttpAggregator.Model;
using Web.Bff.shopping.HttpAggregator.Services;

namespace Web.Bff.shopping.HttpAggregator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        public UserController(UserService userService)
        {
            _userService = userService;
        }
        [Authorize]
        [HttpPost("GetCuentClaim")]
        public async Task<IActionResult> GetCuentClaim()
        {
            return Ok(await _userService.GetCuentClaim()) ;
        }

        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserModel createUserModel)
        {
            try
            {
               
                return Ok(await _userService.CreateaUser(createUserModel));
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
                
                return Ok(await _userService.SignIn(loginUserModel));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}