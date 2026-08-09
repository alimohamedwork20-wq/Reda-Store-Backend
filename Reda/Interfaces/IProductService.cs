using Reda.Entities;

namespace Reda.Interfaces
{
    public interface IProductService
    {
        Task<List<Product>> GetProductsByCategoryAsync(string slug);
        Task<Product> GetProductByIdAsync(int id);
        Task<string> AddProductToCart(int productId,int IdUser);
        Task<List<Product>> GetProductsInCart(int IdUser);
        Task<string> DeleteProductInCart(int IdUser,int IdProduct);
        Task<string> DeleteAllProductsInCart(int idUser);
        Task<string> AddProductToFavorite(int productId, int IdUser);
        Task<string> DeleteProductFromFavorite(int productId, int IdUser);
        Task<List<Product>> GetProductFromFavorite(int UserId);
        Task<List<Product>> SearchProduct(string name);
    }
}
