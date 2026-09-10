using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reda.Dtos;
using Reda.Entities;
using Reda.Helpers;
using Reda.Interfaces;

namespace Reda.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminServices _webServices;

        public AdminController(IAdminServices webServices)
        {
            _webServices = webServices;
        }

        [HttpGet("get-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _webServices.GetAllUsersAsync();

            return Ok(users);
        }

        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto userDto)
        {
            var result =
                await _webServices.UpdateUserAsync(userDto);

            return Ok(result);
        }

        [HttpDelete("delete-user/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result =
                await _webServices.DeleteUserAsync(id);

            return Ok(result);
        }

        [HttpPost("add-user")]
        public async Task<IActionResult> AddUser([FromBody] AddUserDto userDto)
        {
            var result =
                await _webServices.AddUserAsync(userDto);

            return Ok(result);
        }

        [HttpGet("get-contacts")]
        public async Task<IActionResult> GetContacts()
        {
            var result =
                await _webServices.GetContactsAsync();

            return Ok(result);
        }

        [HttpDelete("delete-contact/{id}")]
        public async Task<IActionResult> DeleteContacts(int id)
        {
            var result =
                await _webServices.DeleteContactAsync(id);

            return Ok(result);
        }

        [HttpPost("read-contact")]
        public async Task<IActionResult> ReadingContact([FromBody] ChangeContactDto model)
        {
            var result =
                await _webServices.ReadingContactAsync(
                    model.IdContact);

            return Ok(result);
        }

        [HttpPost("unread-contact")]
        public async Task<IActionResult> UnReadingContact([FromBody] ChangeContactDto model)
        {
            var result =
                await _webServices.UnReadingContactAsync(
                    model.IdContact);

            return Ok(result);
        }

        [HttpPost("reply-contact")]
        public async Task<IActionResult> ReplyContact([FromBody] ChangeContactDto model)
        {
            var result =
                await _webServices.ReplyContactAsync(
                    model.IdContact,
                    model.messageReply);

            return Ok(result);
        }

        [HttpGet("get-all-reports")]
        public async Task<IActionResult> GetAllReports()
        {
            var reports =
                await _webServices.GetAllReportsAsync();

            return Ok(reports);
        }

        [HttpPost("accept-report/{id}")]
        public async Task<IActionResult> AcceptReport(int id)
        {
            await _webServices.AcceptReportAsync(id);

            return Ok(new
            {
                message = "تم قبول البلاغ بنجاح"
            });
        }

        [HttpPost("reject-report/{id}")]
        public async Task<IActionResult> RejectReport(int id)
        {
            await _webServices.RejectReportAsync(id);

            return Ok(new
            {
                message = "تم رفض البلاغ بنجاح"
            });
        }

        [HttpDelete("delete-report/{id}")]
        public async Task<IActionResult> DeleteReport(int id)
        {
            await _webServices.DeleteReportAsync(id);

            return Ok(new
            {
                message = "تم حذف البلاغ بنجاح"
            });
        }
    }
}