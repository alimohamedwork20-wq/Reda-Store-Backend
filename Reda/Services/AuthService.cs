using Microsoft.EntityFrameworkCore;
using Reda.Data;
using Reda.Dtos;
using Reda.Entities;
using Reda.Interfaces;

namespace Reda.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _token;
        private readonly ISendCodeToEmail _sendCodeToEmail;

        public AuthService(AppDbContext context, ITokenService token, ISendCodeToEmail sendCodeToEmail)
        {
            _context = context;
            _token = token;
            _sendCodeToEmail = sendCodeToEmail;
        }

        public async Task<object> LoginAsync(string EmailOrPhone, string password)
        {
            EmailOrPhone = EmailOrPhone?.Trim();
            password = password?.Trim();

            if (string.IsNullOrEmpty(EmailOrPhone) || string.IsNullOrEmpty(password))
                return null;

            bool isEmail = IsValidEmail(EmailOrPhone);
            User user;

            if (isEmail)
            {
                user = await _context.Users.FirstOrDefaultAsync(u => u.Email == EmailOrPhone);
            }
            else
            {
                string phoneDigits = new string(EmailOrPhone.Where(char.IsDigit).ToArray());
                user = await _context.Users.FirstOrDefaultAsync(u => u.Phone == phoneDigits);
            }

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            var token = _token.CreateToken(user);
            return new
            {
                Token = token,
                Name = user.Name,
                Role = user.Role,
                Email = user.Email,
                Id = user.Id,
                Phone = user.Phone,
                Avatar = user.ProfileImageUrl,
                Two_Factor = user.TwoFactor,
            };
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public async Task<User> RegisterAsync(RegisterDto model)
        {
            var exists = await _context.Users.AnyAsync(u => u.Email == model.Email);
            if (exists) throw new InvalidOperationException("Email is already registered.");

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
            var newUser = new User
            {
                Name = model.Name,
                Email = model.Email,
                PasswordHash = hashedPassword
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return newUser;
        }

        public async Task<string> SendCodeToEmailAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return null;
            return await _sendCodeToEmail.SendCodeToEmailAsync(email);
        }

        public async Task<string> CheckOtpToChangePasswordAsync(CheckOtpToChangePasswordDto model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null) return "user not found";

            var code = await _context.Otps
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => o.Code)
                .FirstOrDefaultAsync();

            if (code != model.Code) return "code invaled";
            return "Verified";
        }

        public async Task<string> ResetPasswordAsync(ResetPasswordDto model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null) return "user not found";

            bool isSamePassword = BCrypt.Net.BCrypt.Verify(model.NewPassword, user.PasswordHash);
            if (isSamePassword) return "The password itself cannot be changed";

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            await _context.SaveChangesAsync();
            return "The password was successfully updated";
        }

        public async Task<string> TurnOnTwoFactorAsync(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return "user not found";

            user.TwoFactor = true;
            await _context.SaveChangesAsync();
            return "Two-factor authentication has been enabled.";
        }

        public async Task<string> TurnOffTwoFactorAsync(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return "user not found";

            user.TwoFactor = false;
            await _context.SaveChangesAsync();
            return "Two-factor authentication has been disabled.";
        }

        public async Task<User> GetCurrentUserAsync(int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}