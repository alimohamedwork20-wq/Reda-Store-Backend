namespace Reda.Dtos
{
    public class ChangeEmailDto
    {
        public int UserId { get; set; }
        public string NewEmail { get; set; }
        public string Code { get; set; }
    }
}
