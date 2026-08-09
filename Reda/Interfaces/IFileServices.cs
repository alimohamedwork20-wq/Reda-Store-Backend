using Reda.Dtos;

namespace Reda.Interfaces
{
    public interface IFileServices
    {
        Task<string> UploadReportAsync(ReportDto reportDto);
        Task<string> UploadToCloudinaryAsync(IFormFile file);
    }
}
