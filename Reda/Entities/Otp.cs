namespace Reda.Entities
{
    public class Otp
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Email { get; set; }
        public bool IsUsed { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
