namespace Reda.Dtos
{
    public class VerifyEmailDto
    {
        public int UserId { get; set; }
        public string NewEmail { get; set; }
        public string Code { get; set; }
    }
}
