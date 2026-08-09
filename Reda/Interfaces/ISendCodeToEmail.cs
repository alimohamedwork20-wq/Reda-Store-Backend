namespace Reda.Interfaces
{
    public interface ISendCodeToEmail
    {
        Task<string> SendCodeToEmailAsync(string email);
    }
}
