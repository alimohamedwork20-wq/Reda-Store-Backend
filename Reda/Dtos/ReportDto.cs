namespace Reda.Dtos
{
    public class ReportDto
    {
        public string Category { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public IFormFile? Screenshot { get; set; }
    }
}