using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reda.Dtos;
using Reda.Helpers;
using Reda.Interfaces;

namespace Reda.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _service;

        public AccountController(IAccountService service)
        {
            _service = service;
        }

        [HttpPost("change-name")]
        public async Task<IActionResult> ChangeName([FromBody] ChangeNameDto model)
        {
            var result = await _service.ChangeNameAsync(User.GetUserId(), model.Name);
            if (result == "An error occurred while changing the name") return NotFound(result);
            return Ok(result);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            var result = await _service.ChangePasswordAsync(User.GetUserId(), model.OldPassword, model.NewPassword);
            if (result == "New password is the same as the old password.") return BadRequest(new { Message = result });
            if (result == "Password changed successfully.") return Ok(new { Message = result });
            return BadRequest(new { Message = result });
        }

        [HttpPost("change-email")]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailDto model)
        {
            var result = await _service.ChangeEmailAsync(User.GetUserId(), model.NewEmail, model.Code);
            if (result == "User not found") return NotFound(result);
            if (result == "Invalid OTP code") return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("add-phone")]
        public async Task<IActionResult> AddPhone([FromBody] AddPhoneDto model)
        {
            var result = await _service.AddPhoneAsync(User.GetUserId(), model.Email, model.Phone, model.Code);
            if (result == "User not found") return NotFound(result);
            if (result == "Invalid OTP code") return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount()
        {
            var result = await _service.DeleteAccountAsync(User.GetUserId());
            if (result == "User not found") return NotFound(result);
            return Ok(result);
        }

        [HttpPost("add-profile-image")]
        public async Task<IActionResult> AddProfileImage(IFormFile image)
        {
            var result = await _service.AddProfileImage(image, User.GetUserId());
            if (result == "User not found") return NotFound(result);
            return Ok(result);

        }

        [HttpDelete("remove-profile-image")]
        public async Task<IActionResult> RemoveProfileImage()
        {
            var result = await _service.RemoveProfileImage(User.GetUserId());
            if (result == "User not found") return NotFound(result);
            return Ok(result);
        }

        [HttpGet("get-user-addresses")]
        public async Task<IActionResult> GetUserAddresses()
        {
            var result = await _service.GetUserAddressesAsync(User.GetUserId());
            return Ok(result);
        }

        [HttpPost("add-address")]
        public async Task<IActionResult> AddAddress([FromBody] AddAddressDto model)
        {
            var result = await _service.AddAddressAsync(model, User.GetUserId());
            if (result == "User not found") return NotFound(result);
            return Ok(result);
        }

        [HttpDelete("delete-address")]
        public async Task<IActionResult> DeleteAddress([FromBody] ChangeAndDeleteAddressDto model)
        {
            var result = await _service.DeleteAddressAsync(model.AddressId, User.GetUserId());
            if (result == "User not found" || result == "Address not found") return NotFound(result);
            return Ok(result);
        }

        [HttpPost("set-default-address")]
        public async Task<IActionResult> SetDefaultAddress([FromBody] ChangeAndDeleteAddressDto model)
        {
            var result = await _service.SetDefaultAddressAsync(model.AddressId, User.GetUserId());
            if (result == "No addresses found for this user" || result == "Address not found") return NotFound(result);
            return Ok(result);
        }
    }
}