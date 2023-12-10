using IdenityApi.Model;
using IdenityApi.Models;
using IdenityApi.Services;
using Microsoft.AspNetCore.Mvc;

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
                    CardNumber = createUserModel.CardNumber,
                    Name = createUserModel.Name,
                    SecurityNumber = createUserModel.SecurityNumber,
                    CardHolderName = createUserModel.CardHolderName
                };
                return Ok(await _userService.CreateaUser(applicationUser, createUserModel.password));
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
                return Ok(await _userService.SignIn(loginUserModel.userName, loginUserModel.Password));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
