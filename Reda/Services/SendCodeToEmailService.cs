using Reda.Data;
using Reda.Interfaces;

namespace Reda.Services
{
    public class SendCodeToEmailService : ISendCodeToEmail
    {
        private readonly INotificationService _notificationService;
        private readonly AppDbContext _context;

        public SendCodeToEmailService(INotificationService notificationService, AppDbContext context)
        {
            _notificationService = notificationService;
            _context = context;
        }
        public async Task<string> SendCodeToEmailAsync(string email,string action)
        {
            string otpCode = new Random().Next(100000, 999999).ToString();
            bool isSent = await _notificationService.SendEmailAsync(email, "OTP Code", $"Your OTP code is: {otpCode}");

            if (!isSent)
            {
                throw new Exception("SMTP Server failed to send the email");
            }
            var context = _context.Otps.Add(new Entities.Otp { Email = email, Code = otpCode, CreatedAt = DateTime.UtcNow, Action = action });
            await _context.SaveChangesAsync();
            return "Code sent successfully";
        }
    }
}
