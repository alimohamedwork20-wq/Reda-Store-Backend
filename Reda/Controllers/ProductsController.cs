using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reda.Dtos;
using Reda.Helpers;
using Reda.Interfaces;

namespace Reda.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("products/category/{category}")]
        public async Task<IActionResult> GetProductByCategory(string category)
        {
            var result = await _productService.GetProductsByCategoryAsync(category);
            return Ok(result);
        }

        [HttpGet("product/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var result = await _productService.GetProductByIdAsync(id);
            if (result != null) return Ok(result);
            return NotFound("Product not found");
        }

        [Authorize]
        [HttpPost("product/{productId}/add-to-cart")]
        public async Task<IActionResult> AddProductToCart(int productId)
        {
            var userId = User.GetUserId();
            var result = await _productService.AddProductToCart(productId, userId);

            if (result == "Product added to cart successfully")
                return Ok(result);

            return BadRequest(result);
        }

        [Authorize]
        [HttpGet("cart")]
        public async Task<IActionResult> GetProductsInCart()
        {
            var userId = User.GetUserId();
            var result = await _productService.GetProductsInCart(userId);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("cart/delete/{productId}")]
        public async Task<IActionResult> DeleteProductInCart(int productId)
        {
            var userId = User.GetUserId();
            var result = await _productService.DeleteProductInCart(userId, productId);

            if (result == "Product removed from cart successfully")
                return Ok(result);

            return BadRequest(result);
        }

        [Authorize]
        [HttpDelete("cart/delete-all")]
        public async Task<IActionResult> DeleteAllProductsInCart()
        {
            var userId = User.GetUserId();
            var result = await _productService.DeleteAllProductsInCart(userId);

            if (result == "All products removed from cart successfully")
                return Ok(result);

            return BadRequest(result);
        }

        [Authorize]
        [HttpPost("product/{productId}/add-to-favorite")]
        public async Task<IActionResult> AddProductToFavorite(int productId)
        {
            var userId = User.GetUserId();
            var result = await _productService.AddProductToFavorite(userId, productId);

            if (result == "Product added to Favorite successfully")
                return Ok(result);

            return BadRequest(result);
        }

        [Authorize]
        [HttpDelete("product/{productId}/remove-from-favorite")]
        public async Task<IActionResult> RemoveProductFromFavorite(int productId)
        {
            var userId = User.GetUserId();
            var result = await _productService.DeleteProductFromFavorite(userId, productId);

            if (result == "Product removed from Favorite successfully")
                return Ok(result);

            return BadRequest(result);
        }

        [Authorize]
        [HttpGet("product/show-favoriteItems")]
        public async Task<IActionResult> GetProductFromFavorite()
        {
            var userId = User.GetUserId();
            var result = await _productService.GetProductFromFavorite(userId);
            return Ok(result);
        }

        [HttpGet("products/search")]
        public async Task<IActionResult> SearchProducts([FromQuery] string? term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return BadRequest("Search term cannot be empty.");

            var products = await _productService.SearchProduct(term);
            return Ok(products);
        }
    }
}