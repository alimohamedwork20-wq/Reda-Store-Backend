using System.ComponentModel.DataAnnotations.Schema;

namespace Reda.Entities
{
    public class Addresses
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string City { get; set; }
        public string Details { get; set; }
        public string Phone { get; set; }
        public bool IsDefault { get; set; } = false;
        public int userId { get; set; }
        [ForeignKey(nameof(userId))]
        public User User { get; set; } = null;
    }
}
