using System.ComponentModel.DataAnnotations.Schema;

namespace Reda.Entities
{
    public class Favorite
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<FavoriteItems> FavoriteItems { get; set; } = new List<FavoriteItems>();
    }
}
