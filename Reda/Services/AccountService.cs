using Microsoft.EntityFrameworkCore;
using Reda.Data;
using Reda.Dtos;
using Reda.Entities;
using Reda.Interfaces;

namespace Reda.Services
{
    public class AccountService : IAccountService
    {
        private readonly AppDbContext _context;
        private readonly IFileServices _fileServices;
        public AccountService(AppDbContext context, IFileServices fileServices)
        {
            _context = context;
            _fileServices = fileServices;
        }
        public async Task<string> ChangePasswordAsync(int id, string oldPassword, string newPassword)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    return "User not found.";
                }
                if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
                {
                    return "Invalid old password.";
                }
                if (!BCrypt.Net.BCrypt.Verify(newPassword, user.PasswordHash))
                {
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                    await _context.SaveChangesAsync();
                    return "Password changed successfully.";
                }
                return "New password is the same as the old password.";
            }
            catch (Exception ex)
            {
                return $"An error occurred while changing the password: {ex.Message}";
            }
        }
        public async Task<string> ChangeNameAsync(int id, string name)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    return "An error occurred while changing the name";
                }
                user.Name = name;
                await _context.SaveChangesAsync();
                return user.Name;

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public async Task<string> ChangeEmailAsync(int id, string email, string code)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    return "User not found";
                }
                var chickCode = await _context.Otps.Where(o => o.Email == user.Email)
                                                   .OrderByDescending(x => x.CreatedAt)
                                                   .FirstOrDefaultAsync();
                if (chickCode == null)
                {
                    return "Not found OTP code";
                }
                if (chickCode.Code != code)
                {
                    return "Invalid OTP code";
                }
                user.Email = email;
                await _context.SaveChangesAsync();
                return user.Email;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public async Task<string> AddPhoneAsync(int id, string email, string phone, string code)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return "User not found";
            }
            var chickCode = await _context.Otps.Where(o => o.Email == user.Email)
                                   .OrderByDescending(x => x.CreatedAt)
                                   .FirstOrDefaultAsync();
            if (chickCode == null)
            {
                return "Not found OTP code";
            }
            if (chickCode.Code != code)
            {
                return "Invalid OTP code";
            }
            user.Phone = phone;
            await _context.SaveChangesAsync();
            return user.Phone;
        }
        public async Task<string> DeleteAccountAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return "User not found";
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return "Account deleted successfully";
        }
        public async Task<string> AddProfileImage(IFormFile image, int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return "User not found";
            }
            var result = await _fileServices.UploadToCloudinaryAsync(image);
            user.ProfileImageUrl = result;
            await _context.SaveChangesAsync();
            return user.ProfileImageUrl;
        }
        public async Task<string> RemoveProfileImage(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return "User not found";
            }
            user.ProfileImageUrl = null;
            await _context.SaveChangesAsync();
            return "Profile image removed successfully";
        }
        public async Task<List<Addresses>> GetUserAddressesAsync(int userId)
        {
            var addresses = await _context.Addresses.Where(a => a.userId == userId).ToListAsync();
            return addresses;
        }
        public async Task<string> AddAddressAsync(AddAddressDto addresses)
        {
            var user = await _context.Users.Include(p => p.Addresses).FirstOrDefaultAsync(u => u.Id == addresses.UserId);
            if (user == null)
            {
                return "User not found";
            }
            var defaultAddress = user.Addresses.FirstOrDefault(u => u.IsDefault);
            if (defaultAddress != null)
            {
                defaultAddress.IsDefault = false;
            }
            user.Addresses.Add(new Addresses
            {
                Title = addresses.Title,
                City = addresses.City,
                Details = addresses.Details,
                Phone = addresses.Phone,
                IsDefault = addresses.IsDefault,
                userId = addresses.UserId
            });
            await _context.SaveChangesAsync();
            return "Addresses added successfully";
        }
        public async Task<string> DeleteAddressAsync(ChangeAndDeleteAddressDto addresses)
        {
            var user = await _context.Users.Include(p => p.Addresses).FirstOrDefaultAsync(u => u.Id == addresses.UserId);
            if (user == null)
            {
                return "User not found";
            }
            var addressToDelete = user.Addresses.FirstOrDefault(u => u.Id == addresses.AddressId);
            if (addressToDelete == null)
            {
                return "Address not found";
            }
            user.Addresses.Remove(addressToDelete);
            await _context.SaveChangesAsync();
            return "Address deleted successfully";
        }
        public async Task<string> SetDefaultAddressAsync(ChangeAndDeleteAddressDto addresses)
        {
            var userAddresses = await _context.Addresses
                                    .Where(a => a.userId == addresses.UserId)
                                    .ToListAsync();
            if (!userAddresses.Any())
            {
                return "No addresses found for this user";
            }
            var currentDefault = userAddresses.FirstOrDefault(u => u.IsDefault == true);
            if (currentDefault != null)
            {
                currentDefault.IsDefault = false;
            }
            var addressToSetDefault = userAddresses.FirstOrDefault(u => u.Id == addresses.AddressId);
            if (addressToSetDefault == null)
            {
                return "Address not found";
            }

            addressToSetDefault.IsDefault = true;
            await _context.SaveChangesAsync();
            return "Address set as default successfully";
        }
    }
}