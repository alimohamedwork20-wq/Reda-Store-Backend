using Microsoft.EntityFrameworkCore;
using Reda.Data;
using Reda.Dtos;
using Reda.Entities;
using Reda.Exceptions;
using Reda.Interfaces;

namespace Reda.Services
{
    public class AccountService : IAccountService
    {
        private readonly AppDbContext _context;
        private readonly IAuthService _authService;
        private readonly IFileServices _fileServices;

        public AccountService(
            AppDbContext context,
            IFileServices fileServices,
            IAuthService authService)
        {
            _context = context;
            _fileServices = fileServices;
            _authService = authService;
        }

        public async Task<string> ChangePasswordAsync(int id,string oldPassword,string newPassword)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                throw new NotFoundException("User not found.");

            if (!BCrypt.Net.BCrypt.Verify(
                oldPassword,
                user.PasswordHash))
            {
                throw new BadRequestException(
                    "Invalid old password.");
            }

            if (BCrypt.Net.BCrypt.Verify(
                newPassword,
                user.PasswordHash))
            {
                throw new BadRequestException(
                    "New password is the same as the old password.");
            }

            user.PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(newPassword);

            await _context.SaveChangesAsync();

            return "Password changed successfully.";
        }

        public async Task<string> ChangeNameAsync(int id,string name)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                throw new NotFoundException("User not found.");

            user.Name = name;

            await _context.SaveChangesAsync();

            return user.Name;
        }

        public async Task<string> ChangeEmailAsync(int id,string email,string code)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                throw new NotFoundException("User not found.");
            var emailExists = await _context.Users.AnyAsync(u => u.Email == email && u.Id != id);

            if (emailExists) throw new BadRequestException("Email is already registered.");
            await _authService.CheckOtpAsync(
                new CheckOtpDto
                {
                    Email = user.Email,
                    Code = code,
                    Action = "changeEmail"
                });

            user.Email = email;

            await _context.SaveChangesAsync();

            return user.Email;
        }

        public async Task<string> AddPhoneAsync(int id,string email,string phone,string code)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                throw new NotFoundException("User not found.");

            await _authService.CheckOtpAsync(
                new CheckOtpDto
                {
                    Email = user.Email,
                    Code = code,
                    Action = "changePhone"
                });
            var phoneExists = await _context.Users.AnyAsync(u => u.Phone == phone && u.Id != id);

            if (phoneExists) throw new BadRequestException("Phone number is already registered.");
            user.Phone = phone;

            await _context.SaveChangesAsync();

            return user.Phone;
        }

        public async Task<string> DeleteAccountAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                throw new NotFoundException("User not found.");

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return "Account deleted successfully.";
        }

        public async Task<string> AddProfileImage(IFormFile image,int userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new NotFoundException("User not found.");

            var result =
                await _fileServices.UploadToCloudinaryAsync(image);

            user.ProfileImageUrl = result;

            await _context.SaveChangesAsync();

            return user.ProfileImageUrl;
        }

        public async Task<string> RemoveProfileImage(int userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new NotFoundException("User not found.");

            user.ProfileImageUrl = null;

            await _context.SaveChangesAsync();

            return "Profile image removed successfully.";
        }

        public async Task<List<Addresses>> GetUserAddressesAsync(int userId)
        {
            return await _context.Addresses
                .Where(a => a.userId == userId)
                .ToListAsync();
        }

        public async Task<string> AddAddressAsync(AddAddressDto address,int userId)
        {
            var user = await _context.Users
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new NotFoundException("User not found.");

            if (address.IsDefault)
            {
                var defaultAddress = user.Addresses
                    .FirstOrDefault(a => a.IsDefault);

                if (defaultAddress != null)
                    defaultAddress.IsDefault = false;
            }

            user.Addresses.Add(new Addresses
            {
                Title = address.Title,
                City = address.City,
                Details = address.Details,
                Phone = address.Phone,
                IsDefault = address.IsDefault,
                userId = userId
            });

            await _context.SaveChangesAsync();

            return "Address added successfully.";
        }

        public async Task<string> DeleteAddressAsync(int addressId,int userId)
        {
            var user = await _context.Users
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new NotFoundException("User not found.");

            var addressToDelete = user.Addresses
                .FirstOrDefault(a => a.Id == addressId);

            if (addressToDelete == null)
                throw new NotFoundException("Address not found.");

            user.Addresses.Remove(addressToDelete);

            await _context.SaveChangesAsync();

            return "Address deleted successfully.";
        }

        public async Task<string> SetDefaultAddressAsync(int addressId,int userId)
        {
            var userAddresses = await _context.Addresses
                .Where(a => a.userId == userId)
                .ToListAsync();

            if (!userAddresses.Any())
                throw new NotFoundException(
                    "No addresses found for this user.");

            var addressToSetDefault = userAddresses
                .FirstOrDefault(a => a.Id == addressId);

            if (addressToSetDefault == null)
                throw new NotFoundException(
                    "Address not found.");

            var currentDefault = userAddresses
                .FirstOrDefault(a => a.IsDefault);

            if (currentDefault != null)
                currentDefault.IsDefault = false;

            addressToSetDefault.IsDefault = true;

            await _context.SaveChangesAsync();

            return "Address set as default successfully.";
        }
        
        public async Task<string> SubmitContactFormAsync(AddContactDto contact)
        {
            var contactEntity = new Contact
            {
                Name = contact.Name,
                Email = contact.Email,
                Message = contact.Message
            };

            await _context.Contacts.AddAsync(contactEntity);
            await _context.SaveChangesAsync();

            return "Contact form submitted successfully.";
        }
    }
}
