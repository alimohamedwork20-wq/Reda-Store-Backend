using Reda.Dtos;
using Reda.Entities;

namespace Reda.Interfaces
{
    public interface IAdminServices
    {
        
        Task<List<UserDto>> GetAllUsersAsync();
        Task<string> UpdateUserAsync(UpdateUserDto userDto);
        Task<string> DeleteUserAsync(int id);
        Task<string> AddUserAsync(AddUserDto user);
        Task<List<Contact>> GetContactsAsync();
        Task<string> DeleteContactAsync(int id);
        Task<bool> ReadingContactAsync(int id);
        Task<bool> UnReadingContactAsync(int id);
        Task<bool> ReplyContactAsync(int id,string messageReply);
        Task<List<Report>> GetAllReportsAsync();
        Task<bool> AcceptReportAsync(int reportId);
        Task<bool> RejectReportAsync(int reportId);
        Task<bool> DeleteReportAsync(int reportId);
    }
}
