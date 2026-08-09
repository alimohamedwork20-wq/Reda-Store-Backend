using System.ComponentModel.DataAnnotations.Schema;

namespace Reda.Entities
{
    public class Cart
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // السلة الواحدة جواها لستة من المنتجات (Items)
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
