namespace Reda.Interfaces
{
    public interface INotificationService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody);
    }
}
