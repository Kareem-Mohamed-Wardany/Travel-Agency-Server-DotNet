using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using Travel_Agency_Server.Dtos;
using Travel_Agency_Server.Errors;
using Travel_Agency_Server.Services.Auth;

namespace Travel_Agency_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAuthService _authService;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager,
            IAuthService authService,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _authService = authService;
            _signInManager = signInManager;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(UserDto),200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Login(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return Unauthorized(new ApiResponse(401, "Invalid Login"));

            var res = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (!res.Succeeded) return Unauthorized(new ApiResponse(401, "Invalid Login"));
            return Ok(new UserDto()
            {
                Id=user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Token = await _authService.CreateTokenAsync(user, _userManager)
            });

        }

        [HttpPost("register")]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(ApiValidationErrorResponse),400)]
        public async Task<IActionResult> Register(RegisterDto model)
        {

            var user = new IdentityUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.Phone
            };
            var res = await _userManager.CreateAsync(user, model.Password);
            if (!res.Succeeded) return BadRequest(new ApiValidationErrorResponse() { Errors = res.Errors.Select(e => e.Description) });
            await _userManager.AddToRoleAsync(user, "User");
            return Created();

        }


        //[Authorize]
        //[HttpGet]
        //public async Task<IActionResult> GetCurrentUser()
        //{

        //    var email = User.FindFirstValue(ClaimTypes.Email);

        //    var user = await _userManager.FindByEmailAsync(email);

        //    return Ok(new UserDto()
        //    {
        //        Id = user.Id,
        //        UserName = user.UserName,
        //        Email = user.Email,
        //        Token = await _authService.CreateTokenAsync(user, _userManager)
        //    });
        //}
    }
}
