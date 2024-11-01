using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ZSports.Core.Interfaces.Services;
using ZSports.Core.ViewModel.User;

namespace ZSports.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        [Authorize]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterViewModel viewModel)
        {
            var user = await _userService.RegisterAsync(viewModel);
            return Ok(user);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync([FromBody] LoginViewModel viewModel)
        {
            var token = await _userService.LoginAsync(viewModel);

            return Ok(token);
        }
    }

}
