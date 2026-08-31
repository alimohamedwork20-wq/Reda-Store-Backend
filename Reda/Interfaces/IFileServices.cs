using Reda.Dtos;

namespace Reda.Interfaces
{
    public interface IFileServices
    {
        Task<string> UploadReportAsync(ReportDto reportDto, int userId);
        Task<string> UploadToCloudinaryAsync(IFormFile file);
    }
}