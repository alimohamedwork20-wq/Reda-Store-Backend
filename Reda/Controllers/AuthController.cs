using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reda.Data;
using Reda.Dtos;
using Reda.Entities;
using Reda.Interfaces;

namespace Reda.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _userService;
        private readonly ISendCodeToEmail _sendCodeToEmailService;
        public AuthController(IAuthService userService, ISendCodeToEmail sendCodeToEmailService, AppDbContext context)
        {
            _userService = userService;
            _sendCodeToEmailService = sendCodeToEmailService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var result = await _userService.LoginAsync(model.Email, model.Password);
            if(result == null)
            {
                return NotFound(new { Message = "Invalid email or password." });
            }
            return Ok(result);
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            try
            {
                var result = await _userService.RegisterAsync(model);
                return Ok(new { User = result.Name, Email = result.Email, Id = result.Id });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred.",
                    Details = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpDto model)
        {
            // 1. التحقق من أن الموديل والإيميل ليسوا بقيمة فارغة
            if (model == null || string.IsNullOrEmpty(model.Email))
            {
                return BadRequest(new { Message = "Email is required." });
            }

            try
            {
                // 2. استدعاء السيرفيس (إذا حدث خطأ هناك، سينتقل الكود فوراً للـ catch بالأسفل)
                var result = await _sendCodeToEmailService.SendCodeToEmailAsync(model.Email);

                return Ok(new { Message = result });
            }
            catch (System.Exception ex)
            {
                // 3. 💥 هنا السحر! سنقوم بطباعة الخطأ التفصيلي في الـ Console الخاص بفيجوال استوديو
                System.Diagnostics.Debug.WriteLine($"=== OTP Service Error ===: {ex.Message}");

                // 4. وإرجاعه أيضاً داخل الـ Response لتراه بعينك في Swagger أو الـ React
                return StatusCode(500, new
                {
                    Message = "Failed to send OTP email.",
                    Detail = ex.Message, // 👈 الخطأ القادم من السيرفيس بالتفصيل
                    InnerException = ex.InnerException?.Message // 👈 أي خطأ داخلي متعلق بالشبكة أو السيرفر
                });
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> SendCodeToEmail([FromBody ] SendOtpTOEmailDto model)
        {
            var result = await _userService.SendCodeToEmailAsync(model.Email);
            if(result == null) return NotFound("user not found");
            return Ok(result);
        }
        [HttpPost("check-code")]
        public async Task<IActionResult> CheckOtpToChangePassword([FromBody] CheckOtpToChangePasswordDto model)
        {
            var result = await _userService.CheckOtpToChangePasswordAsync(model);
            if (result == "user not found") return NotFound(result);
            if (result == "code invaled") return BadRequest(result);
            return Ok(result);
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model) 
        {
            try
            {
                var result = await _userService.ResetPasswordAsync(model);
                if (result == "user not found") return NotFound(result);
                if (result == "The password itself cannot be changed") return BadRequest(result);
                return Ok(result);
            }
            catch (Exception ex) { return BadRequest(ex); }
           
        }
        [HttpPost("turn-on-two-factor")]
        public async Task<IActionResult> TurnOnTwoFactor([FromBody] UserIdDto model)
        {
            var result = await _userService.TurnOnTwoFactorAsync(model);
            if (result == "user not found") return NotFound(result);
            return Ok(result);
        }
        [HttpPost("turn-off-two-factor")]
        public async Task<IActionResult> TurnOffTwoFactor([FromBody] UserIdDto model)
        {
            var result = await _userService.TurnOffTwoFactorAsync(model);
            if (result == "user not found") return NotFound(result);
            return Ok(result);
        }
        [HttpGet("get-user")]
        public async Task<IActionResult> GetUserById([FromQuery] UserIdDto model)
        {
            var user = await _userService.GetUserById(model);
            if (user == null) return NotFound("User not found");
            return Ok(user);
        }
    }
    
}
