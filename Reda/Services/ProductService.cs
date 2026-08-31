using Microsoft.EntityFrameworkCore;
using Reda.Data;
using Reda.Entities;
using Reda.Interfaces;

namespace Reda.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(string slug)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.Category.Slug == slug)
                .ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<string> AddProductToCart(int productId, int idUser)
        {
            if (!await _context.Users.AnyAsync(u => u.Id == idUser))
                return "User not found";

            if (!await _context.Products.AnyAsync(p => p.Id == productId))
                return "Product not found";

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == idUser);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = idUser,
                    CartItems = new List<CartItem>()
                };
                _context.Carts.Add(cart);
            }

            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    ProductId = productId,
                    Quantity = 1
                });
            }

            await _context.SaveChangesAsync();
            return "Product added to cart successfully";
        }

        public async Task<List<Product>> GetProductsInCart(int idUser)
        {
            var cart = await _context.Carts
                .AsNoTracking()
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == idUser);

            if (cart == null)
                return new List<Product>();

            return cart.CartItems
                .Where(ci => ci.Product != null)
                .Select(ci => ci.Product)
                .ToList();
        }

        public async Task<string> DeleteProductInCart(int idUser, int idProduct)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == idUser);

            if (cart == null)
                return "Cart not found for this user";

            var itemToRemove = cart.CartItems.FirstOrDefault(ci => ci.ProductId == idProduct);
            if (itemToRemove == null)
                return "Product not found in your cart";

            cart.CartItems.Remove(itemToRemove);
            await _context.SaveChangesAsync();

            return "Product removed from cart successfully";
        }

        public async Task<string> DeleteAllProductsInCart(int idUser)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == idUser);

            if (cart == null)
                return "Cart not found for this user";

            if (cart.CartItems.Count == 0)
                return "Cart is already empty";

            cart.CartItems.Clear();
            await _context.SaveChangesAsync();

            return "All products removed from cart successfully";
        }

        public async Task<string> AddProductToFavorite(int idUser, int idProduct)
        {
            if (!await _context.Users.AnyAsync(u => u.Id == idUser))
                return "User not found";

            if (!await _context.Products.AnyAsync(p => p.Id == idProduct))
                return "Product not found";

            var favorite = await _context.Favorites
                .Include(f => f.FavoriteItems)
                .FirstOrDefaultAsync(f => f.UserId == idUser);

            if (favorite == null)
            {
                favorite = new Favorite
                {
                    UserId = idUser,
                    FavoriteItems = new List<FavoriteItems>()
                };
                _context.Favorites.Add(favorite);
            }

            if (favorite.FavoriteItems.Any(x => x.ProductId == idProduct))
                return "Product is already in favorites";

            favorite.FavoriteItems.Add(new FavoriteItems
            {
                ProductId = idProduct
            });

            await _context.SaveChangesAsync();
            return "Product added to Favorite successfully";
        }

        public async Task<string> DeleteProductFromFavorite(int idUser, int idProduct)
        {
            var favorite = await _context.Favorites
                .Include(f => f.FavoriteItems)
                .FirstOrDefaultAsync(f => f.UserId == idUser);

            if (favorite == null)
                return "Favorite list not found for this user";

            var item = favorite.FavoriteItems.FirstOrDefault(x => x.ProductId == idProduct);
            if (item == null)
                return "Product not found in favorites";

            favorite.FavoriteItems.Remove(item);
            await _context.SaveChangesAsync();

            return "Product removed from Favorite successfully";
        }

        public async Task<List<Product>> GetProductFromFavorite(int userId)
        {
            var favorite = await _context.Favorites
                .AsNoTracking()
                .Include(f => f.FavoriteItems)
                .ThenInclude(fi => fi.Product)
                .FirstOrDefaultAsync(f => f.UserId == userId);

            if (favorite == null)
                return new List<Product>();

            return favorite.FavoriteItems
                .Where(fi => fi.Product != null)
                .Select(fi => fi.Product)
                .ToList();
        }

        public async Task<List<Product>> SearchProduct(string name)
        {
            return await _context.Products
                .Where(p => p.Title.Contains(name))
                .ToListAsync();
        }
    }
}