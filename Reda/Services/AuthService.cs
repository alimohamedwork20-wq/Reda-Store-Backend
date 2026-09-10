using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reda.Data;
using Reda.Dtos;
using Reda.Entities;
using Reda.Exceptions;
using Reda.Interfaces;

namespace Reda.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _token;
        private readonly ISendCodeToEmail _sendCodeToEmail;
        private readonly IValidator<RegisterDto> _validatorR;
        private readonly IValidator<LoginDto> _validatorL;
        public AuthService(
            AppDbContext context,
            ITokenService token,
            ISendCodeToEmail sendCodeToEmail,   
            IValidator<RegisterDto> validatorR,
            IValidator<LoginDto> validatorL
            )
        {
            _context = context;
            _token = token;
            _sendCodeToEmail = sendCodeToEmail;
            _validatorR = validatorR;
            _validatorL = validatorL;
        }

        public async Task<object> LoginAsync(LoginDto model)
        {
            await _validatorL.ValidateAndThrowAsync(model);
            model.EmailOrPhone = model.EmailOrPhone?.Trim();
            model.Password = model.Password?.Trim();

            if (string.IsNullOrEmpty(model.EmailOrPhone) ||
                string.IsNullOrEmpty(model.Password))
            {
                throw new UnauthorizedException(
                    "Invalid email/phone or password.");
            }

            bool isEmail = IsValidEmail(model.EmailOrPhone);

            User user;

            if (isEmail)
            {
                user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == model.EmailOrPhone);
            }
            else
            {
                string phoneDigits = new string(
                    model.EmailOrPhone.Where(char.IsDigit).ToArray()
                );

                user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Phone == phoneDigits);
            }

            if (user == null ||
                !BCrypt.Net.BCrypt.Verify(
                    model.Password,
                    user.PasswordHash))
            {
                throw new UnauthorizedException(
                    "Invalid email/phone or password.");
            }
            if(user.Status == false)
            {
                throw new UnauthorizedException(
                    "Your account is inactive. Please contact support.");
            }
            var token = _token.CreateToken(user);

            return new
            {
                Token = token,
                Name = user.Name,
                Role = user.Role,
                Email = user.Email,
                Phone = user.Phone,
                Avatar = user.ProfileImageUrl,
                Two_Factor = user.TwoFactor,
                Status = user.Status
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
            await _validatorR.ValidateAndThrowAsync(model);
            var exists = await _context.Users
                .AnyAsync(u => u.Email == model.Email);

            if (exists)
                throw new BadRequestException(
                    "Email is already registered.");
            
            string hashedPassword =
                BCrypt.Net.BCrypt.HashPassword(model.Password);

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

        public async Task<string> SendCodeToEmailAsync(string email,string action)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                throw new NotFoundException("User not found.");

            return await _sendCodeToEmail
                .SendCodeToEmailAsync(email, action);
        }

        public async Task<bool> CheckOtpAsync(CheckOtpDto model)
        {
            var code = await _context.Otps
                .Where(o => o.Email == model.Email && o.Action == model.Action)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();

            if (code == null ||
                code.Code != model.Code ||
                code.IsUsed || DateTime.UtcNow > code.CreatedAt.AddMinutes(10))
            {
                throw new BadRequestException(
                    "Invalid OTP code.");
            }
            code.IsUsed = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> ResetPasswordAsync(ResetPasswordDto model)
        {
            var code = await _context.Otps
                            .Where(o => o.Email == model.Email && o.Action == "resetPassword")
                            .OrderByDescending(o => o.CreatedAt)
                            .FirstOrDefaultAsync();
            if(code == null || !code.IsUsed)
            {
                throw new BadRequestException("Invalid");
            }
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null)
                throw new NotFoundException("User not found.");

            bool isSamePassword =
                BCrypt.Net.BCrypt.Verify(
                    model.NewPassword,
                    user.PasswordHash);

            if (isSamePassword)
                throw new BadRequestException(
                    "The new password cannot be the same as the old password.");

            user.PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            _context.Otps.Remove(code);
            await _context.SaveChangesAsync();

            return "The password was successfully updated.";
        }

        public async Task<string> TurnOnTwoFactorAsync(int userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new NotFoundException("User not found.");

            user.TwoFactor = true;

            await _context.SaveChangesAsync();

            return "Two-factor authentication has been enabled.";
        }

        public async Task<string> TurnOffTwoFactorAsync(int userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new NotFoundException("User not found.");

            user.TwoFactor = false;

            await _context.SaveChangesAsync();

            return "Two-factor authentication has been disabled.";
        }

        public async Task<object> GetCurrentUserAsync(int userId)
        {
            var user = await _context.Users.Select(u=> new { u.Id, u.Name, u.Email, u.TwoFactor,u.Status,u.ProfileImageUrl,u.Role,u.Phone}).FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new NotFoundException("User not found.");

            return user;
        }
    }
}