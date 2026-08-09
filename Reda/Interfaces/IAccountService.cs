using Reda.Dtos;
using Reda.Entities;

namespace Reda.Interfaces
{
    public interface IAccountService
    {
        Task<string> ChangePasswordAsync(int id, string oldPassword, string newPassword);
        Task<string> ChangeNameAsync(int id, string name);
        Task<string> ChangeEmailAsync(int id, string email,string code);
        Task<string> AddPhoneAsync(int id, string email, string phone, string code);
        Task<string> DeleteAccountAsync(int id);
        Task<string> AddProfileImage(IFormFile image, int userId);
        Task<string> RemoveProfileImage(int userId);
        Task<List<Addresses>> GetUserAddressesAsync(int userId);
        Task<string> AddAddressAsync(AddAddressDto addresses);
        Task<string> DeleteAddressAsync(ChangeAndDeleteAddressDto addresses);
        Task<string> SetDefaultAddressAsync(ChangeAndDeleteAddressDto addresses);
    }
}
