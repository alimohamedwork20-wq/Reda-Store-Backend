using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reda.Dtos;
using Reda.Interfaces;

namespace Reda.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        public IAccountService _service { get; set; }
        public INotificationService _notificationService { get; set; }
        public AccountController(IAccountService service, INotificationService notificationService)
        {
            _service = service;
            _notificationService = notificationService;
        }

        [Authorize]
        [HttpPost("change-name")]
        public async Task<IActionResult> ChangeName([FromBody] ChangeNameDto model)
        {
            var result = await _service.ChangeNameAsync(model.UserId, model.Name);
            if (result == "An error occurred while changing the name")
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            var result = await _service.ChangePasswordAsync(model.UserId, model.OldPassword, model.NewPassword);
            if (result == "New password is the same as the old password.")
            {
                return BadRequest(new { Message = result });
            }
            if (result == "Password changed successfully.")
            {
                return Ok(new { Message = result });
            }
            return BadRequest(new { Message = result });
        }

        [Authorize]
        [HttpPost("change-email")]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailDto model)
        {
            var result = await _service.ChangeEmailAsync(model.UserId, model.NewEmail, model.Code);
            if (result == "User not found")
            {
                return NotFound(result);
            }
            if (result == "Invalid OTP code")
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [Authorize]
        [HttpPost("add-phone")]
        public async Task<IActionResult> AddPhone([FromBody] AddPhoneDto model)
        {
            var result = await _service.AddPhoneAsync(model.UserId, model.Email, model.Phone, model.Code);
            if (result == "User not found")
            {
                return NotFound(result);
            }
            if (result == "Invalid OTP code")
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount([FromBody] UserIdDto model)
        {
            var result = await _service.DeleteAccountAsync(model.UserId);
            if (result == "User not found")
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [Authorize]
        [HttpPost("add-profile-image/{id}")]
        public async Task<IActionResult> AddProfileImage(int id, IFormFile image)
        {
            var result = await _service.AddProfileImage(image, id);
            if (result == "User not found")
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("remove-profile-image")]
        public async Task<IActionResult> RemoveProfileImage([FromBody] UserIdDto model)
        {
            var result = await _service.RemoveProfileImage(model.UserId);
            if (result == "User not found")
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [Authorize]
        [HttpGet("get-user-addresses")]
        public async Task<IActionResult> GetUserAddresses([FromQuery] UserIdDto model)
        {
            var result = await _service.GetUserAddressesAsync(model.UserId);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("add-address")]
        public async Task<IActionResult> AddAddress([FromBody] AddAddressDto model)
        {
            var result = await _service.AddAddressAsync(model);
            if (result == "User not found")
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("delete-address")]
        public async Task<IActionResult> DeleteAddress([FromBody] ChangeAndDeleteAddressDto model)
        {
            var result = await _service.DeleteAddressAsync(model);
            if (result == "User not found")
            {
                return NotFound(result);
            }
            if (result == "Address not found")
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [Authorize]
        [HttpPost("set-default-address")]
        public async Task<IActionResult> SetDefaultAddress([FromBody] ChangeAndDeleteAddressDto model)
        {
            var result = await _service.SetDefaultAddressAsync(model);
            if (result == "No addresses found for this user")
            {
                return NotFound(result);
            }
            if (result == "Address not found")
            {
                return NotFound(result);
            }
            return Ok(result);

        }
    }
}