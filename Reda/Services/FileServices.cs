using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Reda.Data;
using Reda.Dtos;
using Reda.Interfaces;

namespace Reda.Services
{
    public class FileServices : IFileServices
    {
        private readonly AppDbContext _context;
        public FileServices(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> UploadReportAsync(ReportDto reportDto, int userId)
        {
            string cloudUrl = null;
            if (reportDto.Screenshot != null && reportDto.Screenshot.Length > 0)
            {
                try
                {
                    cloudUrl = await UploadToCloudinaryAsync(reportDto.Screenshot);
                }
                catch (Exception ex) { return $"Error uploading file: {ex.Message}"; }
            }

            var report = new Entities.Report
            {
                Category = reportDto.Category,
                Subject = reportDto.Subject,
                Description = reportDto.Description,
                Screenshot = cloudUrl,
                UserId = userId
            };

            _context.Reports.Add(report);
            await _context.SaveChangesAsync();
            return "Saved successfully";
        }

        public async Task<string> UploadToCloudinaryAsync(IFormFile file)
        {
            // TODO: move Cloudinary credentials to configuration/user secrets/environment variables.
            var account = new Account(
                "dbd8q7vsm",
                "884356529482583",
                "NTZD2dYfmFE-MgkTddiHO8xTFaY"
            );

            var cloudinary = new Cloudinary(account);
            using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "problem_reports"
            };

            var uploadResult = await cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                return uploadResult.SecureUrl.ToString();

            throw new Exception(uploadResult.Error?.Message ?? "Unknown Cloudinary error");
        }
    }
}