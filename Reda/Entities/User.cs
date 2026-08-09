using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace Reda.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? Phone {  get; set; }
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = "User";
        public bool Status { get; set; } = true;
        public string? ProfileImageUrl { get; set; }
        [DefaultValue(false)]
        public bool TwoFactor { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Report> Reports { get; set; } = new List<Report>();
        public ICollection<Addresses> Addresses { get; set; } = new List<Addresses>();
    }
}
