using Microsoft.EntityFrameworkCore;
using Reda.Data;
using Reda.Dtos;
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

        public async Task<string> AddProductToCart(int productId, int userId)
        {
            if (!await _context.Users.AnyAsync(u => u.Id == userId))
                return "User not found";

            if (!await _context.Products.AnyAsync(p => p.Id == productId))
                return "Product not found";

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CartItems = new List<CartItem>()
                };
                _context.Carts.Add(cart);
            }

            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

            if (existingItem != null)
                existingItem.Quantity++;
            else
                cart.CartItems.Add(new CartItem { ProductId = productId, Quantity = 1 });

            await _context.SaveChangesAsync();
            return "Product added to cart successfully";
        }

        public async Task<List<CartProductDto>> GetProductsInCart(int userId)
        {
            var cart = await _context.Carts
                .AsNoTracking()
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
                return new List<CartProductDto>();

            return cart.CartItems
                .Where(ci => ci.Product != null)
                .Select(ci => new CartProductDto
                {
                    Id = ci.Product.Id,
                    Title = ci.Product.Title,
                    Description = ci.Product.Description,
                    Price = ci.Product.Price,
                    Rating = ci.Product.Rating,
                    Stock = ci.Product.Stock,
                    Brand = ci.Product.Brand,
                    Thumbnail = ci.Product.Thumbnail,
                    Quantity = ci.Quantity
                })
                .ToList();
        }

        public async Task<string> UpdateCartQuantity(int userId, int productId, int quantity)
        {
            if (quantity < 1)
                return "Quantity must be at least 1";

            var cartItem = await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.Cart.UserId == userId && ci.ProductId == productId);

            if (cartItem == null)
                return "Product not found in your cart";

            cartItem.Quantity = quantity;
            await _context.SaveChangesAsync();

            return "Cart quantity updated successfully";
        }

        public async Task<string> DeleteProductInCart(int userId, int productId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
                return "Cart not found for this user";

            var itemToRemove = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (itemToRemove == null)
                return "Product not found in your cart";

            cart.CartItems.Remove(itemToRemove);
            await _context.SaveChangesAsync();
            return "Product removed from cart successfully";
        }

        public async Task<string> DeleteAllProductsInCart(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
                return "Cart not found for this user";

            if (cart.CartItems.Count == 0)
                return "Cart is already empty";

            cart.CartItems.Clear();
            await _context.SaveChangesAsync();
            return "All products removed from cart successfully";
        }

        public async Task<string> AddProductToFavorite(int userId, int productId)
        {
            if (!await _context.Users.AnyAsync(u => u.Id == userId))
                return "User not found";

            if (!await _context.Products.AnyAsync(p => p.Id == productId))
                return "Product not found";

            var favorite = await _context.Favorites
                .Include(f => f.FavoriteItems)
                .FirstOrDefaultAsync(f => f.UserId == userId);

            if (favorite == null)
            {
                favorite = new Favorite
                {
                    UserId = userId,
                    FavoriteItems = new List<FavoriteItems>()
                };
                _context.Favorites.Add(favorite);
            }

            if (favorite.FavoriteItems.Any(x => x.ProductId == productId))
                return "Product is already in favorites";

            favorite.FavoriteItems.Add(new FavoriteItems { ProductId = productId });
            await _context.SaveChangesAsync();
            return "Product added to Favorite successfully";
        }

        public async Task<string> DeleteProductFromFavorite(int userId, int productId)
        {
            var favorite = await _context.Favorites
                .Include(f => f.FavoriteItems)
                .FirstOrDefaultAsync(f => f.UserId == userId);

            if (favorite == null)
                return "Favorite list not found for this user";

            var item = favorite.FavoriteItems.FirstOrDefault(x => x.ProductId == productId);
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