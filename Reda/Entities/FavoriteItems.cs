using System.ComponentModel.DataAnnotations.Schema;

namespace Reda.Entities
{
    public class FavoriteItems
    {
        public int Id { get; set; }

        public int FavoriteId { get; set; }
        [ForeignKey(nameof(FavoriteId))]
        public Favorite Favorite { get; set; } = null!;

        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = null!;
    }
}
