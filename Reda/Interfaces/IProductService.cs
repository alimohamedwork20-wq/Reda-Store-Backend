using Reda.Dtos;
using Reda.Entities;

namespace Reda.Interfaces
{
    public interface IProductService
    {
        Task<List<Product>> GetProductsByCategoryAsync(string slug);
        Task<Product> GetProductByIdAsync(int id);
        Task<string> AddProductToCart(int productId, int userId);
        Task<List<CartProductDto>> GetProductsInCart(int userId);
        Task<string> UpdateCartQuantity(int userId, int productId, int quantity);
        Task<string> DeleteProductInCart(int userId, int productId);
        Task<string> DeleteAllProductsInCart(int userId);
        Task<string> AddProductToFavorite(int userId, int productId);
        Task<string> DeleteProductFromFavorite(int userId, int productId);
        Task<List<Product>> GetProductFromFavorite(int userId);
        Task<List<Product>> SearchProduct(string name);
    }
}