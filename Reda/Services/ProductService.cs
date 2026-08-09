using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reda.Data;
using Reda.Entities;
using Reda.Interfaces;
using System.Linq;
using System.Text.Json;
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
            var result = await _context.Products.Include(p => p.Category)
                                                .Where(p => p.Category.Slug == slug)
                                                .ToListAsync();
            if (result.Count != 0)
            {
                return result;
            }
            return null;
        }
        public async Task<Product> GetProductByIdAsync(int id)
        {
            var result = await _context.Products.Include(p => p.Category).Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);
            if (result != null)
            {
                return result;
            }
            return null;
        }
        public async Task<string> AddProductToCart(int productId, int IdUser)
        {
            if (IdUser == null)
            {
                return "User not found";
            }
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return "Product not found";
            }
            var cart = await _context.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.UserId == IdUser);
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = IdUser,
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
                var newItem = new CartItem
                {
                    ProductId = productId,
                    Quantity = 1
                };
                cart.CartItems.Add(newItem);
            }
            await _context.SaveChangesAsync();
            return "Product added to cart successfully";
        }
        public async Task<List<Product>> GetProductsInCart(int IdUser)
        {
            var cart = await _context.Carts.Include(c => c.CartItems)
                                           .ThenInclude(ci => ci.Product)
                                           .FirstOrDefaultAsync(c => c.UserId == IdUser);
            if (cart != null && cart.CartItems.Count > 0)
            {
                return cart.CartItems.Select(ci => ci.Product).ToList();
            }
            return new List<Product>();
        }
        public async Task<string> DeleteProductInCart(int IdUser, int IdProduct)
        {
            var cart = await _context.Carts
                                         .Include(c => c.CartItems)
                                         .FirstOrDefaultAsync(c => c.UserId == IdUser);
            if (cart == null)
            {
                return "Cart not found for this user";
            }
            var itemToRemove = cart.CartItems.FirstOrDefault(ci => ci.ProductId == IdProduct);
            if (itemToRemove == null)
            {
                return "Product not found in your cart";
            }
            cart.CartItems.Remove(itemToRemove);
            await _context.SaveChangesAsync();

            return "Product removed from cart successfully";

        }
        public async Task<string> DeleteAllProductsInCart(int IdUser)
        {
            var cart = await _context.Carts
                             .Include(c => c.CartItems)
                             .FirstOrDefaultAsync(c => c.UserId == IdUser);

            if (cart == null)
            {
                return "Cart not found for this user";
            }

            // 2. التحقق مما إذا كانت السلة تحتوي على عناصر بالفعل
            if (cart.CartItems.Count == 0)
            {
                return "Cart is already empty";
            }
            cart.CartItems.Clear();
            await _context.SaveChangesAsync();

            return "All products removed from cart successfully";
        }
        public async Task<string> AddProductToFavorite(int IdUser, int IdProduct)
        {
            if (IdUser == null)
            {
                return "User not found";
            }
            var product = await _context.Products.FindAsync(IdProduct);
            if (product == null)
            {
                return "Product not found";
            }
            var favorite = await _context.Favorites.Include(f => f.FavoriteItems).FirstOrDefaultAsync(u => u.UserId == IdUser);
            bool exists = favorite.FavoriteItems.Any(x => x.ProductId == IdProduct);
            if (exists)
            {
                throw new InvalidOperationException("Product is already in favorites.");
            }
            if (favorite == null)
            {
                favorite = new Favorite
                {
                    UserId = IdUser,
                    FavoriteItems = new List<FavoriteItems>()
                };
                _context.Favorites.Add(favorite);
            }
            var newItem = new FavoriteItems
            {
                ProductId = IdProduct,
            };

            favorite.FavoriteItems.Add(newItem);
            await _context.SaveChangesAsync();
            return "Product added to Favorite successfully";
        }
        public async Task<string> DeleteProductFromFavorite(int IdUser, int IdProduct)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u=> u.Id == IdUser);
            if (user == null)  return "User Not Found";
            var product = await _context.Products.FirstOrDefaultAsync(p=> p.Id == IdProduct);
            var favorIteItems = await _context.Favorites.Include(f => f.FavoriteItems).FirstOrDefaultAsync(u => u.UserId == IdUser);
            var productInFav =  favorIteItems.FavoriteItems.FirstOrDefault(p=> p.ProductId == IdProduct);
            favorIteItems.FavoriteItems.Remove(productInFav);
             await _context.SaveChangesAsync();
            return "Product removed from Favorite successfully";
            
        }
        public async Task<List<Product>> GetProductFromFavorite(int UserId)
        {
            var favforiteItems = await _context.Favorites.Include(c => c.FavoriteItems ).ThenInclude(c=> c.Product).FirstOrDefaultAsync(u => u.UserId == UserId);
            if(favforiteItems != null) return favforiteItems.FavoriteItems.Select(fi => fi.Product).ToList(); ;
            return null;

        }
        public async Task<List<Product>> SearchProduct(string name) 
        {
            var products = await _context.Products.Where(p=> p.Title.Contains(name)).ToListAsync();
            return products;
        }

    }
}