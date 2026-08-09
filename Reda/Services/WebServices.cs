using Microsoft.EntityFrameworkCore;
using Reda.Data;
using Reda.Dtos;
using Reda.Entities;
using Reda.Interfaces;
namespace Reda.Services
{
    public class WebServices : IWebServices
    {
        private readonly AppDbContext _context;
        public WebServices(AppDbContext context)
        {
            _context = context;
        }
        public async Task<string> SubmitContactFormAsync(Contact contact)
        {
            await _context.Contacts.AddAsync(contact);
            await _context.SaveChangesAsync();
            return "Contact form submitted successfully.";
        }
        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            return await _context.Users.Select(u => new UserDto { Id = u.Id, Name = u.Name, Email = u.Email, Role = u.Role, Status = u.Status }).ToListAsync();
        }
        public async Task<string> UpdateUserAsync(UserDto userDto)
        {
            var user = await _context.Users.FindAsync(userDto.Id);
            if (user == null)
            {
                return "User not found.";
            }
            if (!string.IsNullOrEmpty(userDto.Name))
            {
                user.Name = userDto.Name;
            }
            if (!string.IsNullOrEmpty(userDto.Email))
            {
                user.Email = userDto.Email;
            }
            if (!string.IsNullOrEmpty(userDto.Role))
            {
                user.Role = userDto.Role;
            }
            user.Status = userDto.Status;
            await _context.SaveChangesAsync();
            return "User updated successfully.";
        }
        public async Task<string> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return "User not found.";
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return "User deleted successfully.";
        }
        public async Task<string> AddUserAsync(UserDto userDto)
        {
            try
            {
                var hashPassword = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
                var user = new User
                {
                    Name = userDto.Name,
                    Email = userDto.Email,
                    Role = userDto.Role,
                    Status = userDto.Status,
                    PasswordHash = hashPassword

                };
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return "User Added succesfuly";
            }
            catch (Exception ex) {
                return $"Error is: ${ex.Message}";
            }
        }
        public async Task<List<Contact>> GetContactsAsync()
        {
            return await _context.Contacts.ToListAsync();
        }
        public async Task<string> DeleteContactAsync(int id) 
        {
            var message = await _context.Contacts.FindAsync(id);
            if (message == null) return "message not found";
            _context.Contacts.Remove(message);
            await _context.SaveChangesAsync();
            return "Deleted Message Successfuly";   
        }
        public async Task<bool> ReadingContactAsync(int id)
        {
            var message = await _context.Contacts.FindAsync(id);
            if (message == null) return false;
            message.IsRead = true;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UnReadingContactAsync(int id)
        {
            var message = await _context.Contacts.FindAsync(id);
            if (message == null) return false;
            message.IsRead = false;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> ReplyContactAsync(int id,string messageReply)
        {
            var message = await _context.Contacts.FindAsync(id);
            if (message == null) return false;
            message.IsReply = messageReply;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<Report>> GetAllReportsAsync()
        {
            return await _context.Reports
                .Include(r => r.User) // لتضمين بيانات المستخدم صاحب البلاغ
                .OrderByDescending(r => r.SentAt)
                .ToListAsync();
        }

        public async Task<bool> AcceptReportAsync(int reportId)
        {
            var report = await _context.Reports.FindAsync(reportId);
            if (report == null) return false;

            report.Status = 1; // 1 = مقبول / معالج
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectReportAsync(int reportId)
        {
            var report = await _context.Reports.FindAsync(reportId);
            if (report == null) return false;

            report.Status = 2; // 2 = مرفوض
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteReportAsync(int reportId)
        {
            var report = await _context.Reports.FindAsync(reportId);
            if (report == null) return false;

            _context.Reports.Remove(report); // بدون await
            await _context.SaveChangesAsync();
            return true;
        }
    }
}