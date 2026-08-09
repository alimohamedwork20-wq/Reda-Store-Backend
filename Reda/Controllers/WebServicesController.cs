using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reda.Dtos;
using Reda.Entities;
using Reda.Interfaces;

namespace Reda.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebServicesController : Controller
    {
        private readonly IWebServices _webServices;
        private readonly IFileServices _fileServices;
        public WebServicesController(IWebServices webServices, IFileServices fileServices)
        {
            _webServices = webServices;
            _fileServices = fileServices;
        }
        [Authorize]
        [HttpPost("submit-contact-form")]
        public async Task<IActionResult> SubmitContactForm([FromBody] Contact contact)
        {
            var result = await _webServices.SubmitContactFormAsync(contact);
            return Ok(result);
        }
        [Authorize]
        [HttpPost("submit-report")]
        public async Task<IActionResult> SubmitReport([FromForm] ReportDto reportDto)
        {
            try
            {
                var result = await _fileServices.UploadReportAsync(reportDto);
                return Ok(result);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
        [Authorize]
        [HttpGet("get-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _webServices.GetAllUsersAsync();
            return Ok(users);
        }
        [Authorize]
        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUser([FromBody] UserDto userDto)
        {
            var result = await _webServices.UpdateUserAsync(userDto);
            if(result == "User not found.")
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        [Authorize]
        [HttpDelete("delete-user/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _webServices.DeleteUserAsync(id);
            if (result == "User not found.")
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        [Authorize]
        [HttpPost("add-user")]
        public async Task<IActionResult> AddUser([FromBody]  UserDto userDto)
        {
            var result = await _webServices.AddUserAsync(userDto);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("get-contacts")]
        public async Task<IActionResult> GetContacts()
        {
            var result = await _webServices.GetContactsAsync();
            return Ok(result);
        }
        [Authorize]
        [HttpDelete("delete-contact/{id}")]
        public async Task<IActionResult> DeleteContacts(int id)
        {
            var result = await _webServices.DeleteContactAsync(id);
            if (result == "message not found") return NotFound(result);
            return Ok(result);
        }
        [Authorize]
        [HttpPost("read-contact")]
        public async Task<IActionResult> ReadingContact([FromBody] ChangeContactDto model)
        {
            var result = await _webServices.ReadingContactAsync(model.IdContact);
            if (result == false) return NotFound(result);
            return Ok(result);
        }
        [Authorize]
        [HttpPost("unread-contact")]
        public async Task<IActionResult> UnReadingContact([FromBody] ChangeContactDto model)
        {
            var result = await _webServices.UnReadingContactAsync(model.IdContact);
            if (result == false) return NotFound(result);
            return Ok(result);
        }
        [Authorize]
        [HttpPost("reply-contact")]
        public async Task<IActionResult> ReplyContact([FromBody] ChangeContactDto model)
        {
            var result = await _webServices.ReplyContactAsync(model.IdContact,model.messageReply);
            if (result == false) return NotFound(result);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("get-all-reports")]
        public async Task<IActionResult> GetAllReports()
        {
            var reports = await _webServices.GetAllReportsAsync();
            return Ok(reports);
        }

        [Authorize]
        [HttpPost("accept-report/{id}")]
        public async Task<IActionResult> AcceptReport(int id)
        {
            var result = await _webServices.AcceptReportAsync(id);
            if (!result) return NotFound("البلاغ غير موجود");
            return Ok(new { message = "تم قبول البلاغ بنجاح" });
        }

        [Authorize]
        [HttpPost("reject-report/{id}")]
        public async Task<IActionResult> RejectReport(int id)
        {
            var result = await _webServices.RejectReportAsync(id);
            if (!result) return NotFound("البلاغ غير موجود");
            return Ok(new { message = "تم رفض البلاغ بنجاح" });
        }

        [Authorize]
        [HttpDelete("delete-report/{id}")]
        public async Task<IActionResult> DeleteReport(int id)
        {
            var result = await _webServices.DeleteReportAsync(id);
            if (!result) return NotFound("البلاغ غير موجود");
            return Ok(new { message = "تم حذف البلاغ بنجاح" });
        }
    }
}