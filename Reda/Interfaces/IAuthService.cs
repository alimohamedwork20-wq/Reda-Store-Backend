using Reda.Dtos;
using Reda.Entities;

namespace Reda.Interfaces
{
    public interface IAuthService
    {
        Task<object> LoginAsync(string email, string password);
        Task<User> RegisterAsync(RegisterDto model);
        Task<string> SendCodeToEmailAsync(string email);
        Task<string> CheckOtpToChangePasswordAsync(CheckOtpToChangePasswordDto model);
        Task<string> ResetPasswordAsync(ResetPasswordDto model);
        Task<string> TurnOnTwoFactorAsync(int userId);
        Task<string> TurnOffTwoFactorAsync(int userId);
        Task<User> GetCurrentUserAsync(int userId);
    }
}