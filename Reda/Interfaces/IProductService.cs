using Reda.Entities;

namespace Reda.Interfaces
{
    public interface IProductService
    {
        Task<List<Product>> GetProductsByCategoryAsync(string slug);
        Task<Product> GetProductByIdAsync(int id);
        Task<string> AddProductToCart(int productId, int idUser);
        Task<List<Product>> GetProductsInCart(int idUser);
        Task<string> UpdateCartQuantity(int idUser, int idProduct, int quantity);
        Task<string> DeleteProductInCart(int idUser, int idProduct);
        Task<string> DeleteAllProductsInCart(int idUser);
        Task<string> AddProductToFavorite(int idUser, int idProduct);
        Task<string> DeleteProductFromFavorite(int idUser, int idProduct);
        Task<List<Product>> GetProductFromFavorite(int userId);
        Task<List<Product>> SearchProduct(string name);
    }
}