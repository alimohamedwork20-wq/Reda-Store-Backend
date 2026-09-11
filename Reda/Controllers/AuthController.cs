using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reda.Dtos;
using Reda.Helpers;
using Reda.Interfaces;

namespace Reda.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _userService;
        private readonly ISendCodeToEmail _sendCodeToEmailService;

        public AuthController(
            IAuthService userService,
            ISendCodeToEmail sendCodeToEmailService)
        {
            _userService = userService;
            _sendCodeToEmailService = sendCodeToEmailService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginDto model)
        {
            var result = await _userService.LoginAsync(model);
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterDto model)
        {
            var result =
                await _userService.RegisterAsync(model);

            return Ok(new
            {
                User = result.Name,
                Email = result.Email
            });
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpDto model)
        {
            if (model == null ||
                string.IsNullOrWhiteSpace(model.Email))
            {
                return BadRequest(new
                {
                    Message = "Email is required."
                });
            }

            var result =
                await _sendCodeToEmailService
                    .SendCodeToEmailAsync(model.Email, model.Action);

            return Ok(new
            {
                Message = result
            });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> SendCodeToEmail(
            [FromBody] SendOtpTOEmailDto model)
        {
            var result =
                await _userService.SendCodeToEmailAsync(
                    model.Email, "resetPassword");

            return Ok(result);
        }

        [HttpPost("check-code")]
        public async Task<IActionResult> CheckOtpToChangePassword(
            [FromBody] CheckOtpDto model)
        {
            var result =
                await _userService.CheckOtpAsync(model);

            return Ok(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(
            [FromBody] ResetPasswordDto model)
        {
            var result =
                await _userService.ResetPasswordAsync(model);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("turn-on-two-factor")]
        public async Task<IActionResult> TurnOnTwoFactor()
        {
            var result =
                await _userService.TurnOnTwoFactorAsync(
                    User.GetUserId());

            return Ok(result);
        }

        [Authorize]
        [HttpPost("turn-off-two-factor")]
        public async Task<IActionResult> TurnOffTwoFactor()
        {
            var result =
                await _userService.TurnOffTwoFactorAsync(
                    User.GetUserId());

            return Ok(result);
        }

        [Authorize]
        [HttpGet("get-user")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user =
                await _userService.GetCurrentUserAsync(
                    User.GetUserId());

            return Ok(user);
        }
    }
}