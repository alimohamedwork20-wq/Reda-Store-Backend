using Reda.Interfaces;
using System.Net;
using System.Net.Mail;

namespace Reda.Services
{
    public class EmailNotificationService : INotificationService
    {
        private readonly IConfiguration _configuration;

        public EmailNotificationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            try
            {
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var port = int.Parse(_configuration["EmailSettings:Port"]);
                var senderEmail = _configuration["EmailSettings:SenderEmail"];
                var senderName = _configuration["EmailSettings:SenderName"];
                var username = _configuration["EmailSettings:Username"];
                var password = _configuration["EmailSettings:Password"];

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, senderName),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(toEmail);

                // 👇 التعديل الجذري والآمن للاتصال بسيرفر جوجل
                using (var smtpClient = new SmtpClient(smtpServer, port))
                {
                    // 1. تصفير أي إعدادات افتراضية مسبقة
                    smtpClient.UseDefaultCredentials = false;

                    // 2. تمرير الإيميل وكلمة مرور التطبيق الـ 16 حرفاً
                    smtpClient.Credentials = new NetworkCredential(username, password);

                    // 3. تفعيل التشفير وهو إلزامي لـ Gmail
                    smtpClient.EnableSsl = true;

                    // 4. تحديد طريقة الإرسال عبر الشبكة مباشرة
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                    // 5. إرسال الرسالة
                    await smtpClient.SendMailAsync(mailMessage);
                }

                return true;
            }
            catch (Exception ex)
            {
                // لكي نرى سبب الرفض الحقيقي من سيرفر جوجل في الـ Detail
                throw new Exception($"Google SMTP Rejected: {ex.Message}", ex);
            }
        }
    }
}
