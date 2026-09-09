using Reda.Dtos;
using Reda.Entities;

namespace Reda.Interfaces
{
    public interface IAuthService
    {
        Task<object> LoginAsync(LoginDto model);
        Task<User> RegisterAsync(RegisterDto model);
        Task<string> SendCodeToEmailAsync(string email);
        Task<bool> CheckOtpAsync(CheckOtpDto model);
        Task<string> ResetPasswordAsync(ResetPasswordDto model);
        Task<string> TurnOnTwoFactorAsync(int userId);
        Task<string> TurnOffTwoFactorAsync(int userId);
        Task<User> GetCurrentUserAsync(int userId);
    }
}