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
        private readonly IConfiguration _configuration;

        public FileServices(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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
                catch (Exception ex)
                {
                    return $"Error uploading file: {ex.Message}";
                }
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
            var cloudName = _configuration["Cloudinary:CloudName"];
            var apiKey = _configuration["Cloudinary:ApiKey"];
            var apiSecret = _configuration["Cloudinary:ApiSecret"];

            if (string.IsNullOrWhiteSpace(cloudName) ||
                string.IsNullOrWhiteSpace(apiKey) ||
                string.IsNullOrWhiteSpace(apiSecret))
            {
                throw new InvalidOperationException("Cloudinary configuration is missing.");
            }

            var account = new Account(cloudName, apiKey, apiSecret);
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