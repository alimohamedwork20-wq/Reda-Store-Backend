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
        Task<string> TurnOnTwoFactorAsync(UserIdDto model);
        Task<string> TurnOffTwoFactorAsync(UserIdDto model);
        Task<User> GetUserById(UserIdDto model);
    }
}
