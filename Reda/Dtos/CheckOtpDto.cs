namespace Reda.Dtos
{
    public class CheckOtpDto
    {
        public string Email { get; set; }
        public string Code { get; set; }
        public string Action { get; set; } = string.Empty;
    }
}
