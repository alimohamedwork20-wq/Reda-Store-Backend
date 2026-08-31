using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reda.Entities
{
    [Index(nameof(CartId), nameof(ProductId), IsUnique = true)]
    public class CartItem
    {
        public int Id { get; set; }

        public int CartId { get; set; }

        [ForeignKey(nameof(CartId))]
        public Cart Cart { get; set; } = null!;

        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = null!;

        public int Quantity { get; set; } = 1;
    }
}
