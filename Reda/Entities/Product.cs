using System.ComponentModel.DataAnnotations.Schema;

namespace Reda.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal Rating { get; set; }
        public int Stock { get; set; }
        public string Brand { get; set; }
        public string Thumbnail { get; set; }
        public int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }
        public ICollection<Image> Images { get; set; } = new List<Image>();

    }
}
