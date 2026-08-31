using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            return result == "Product added to cart successfully" ? Ok(result) : BadRequest(result);
        }

        [Authorize]
        [HttpGet("cart")]
        public async Task<IActionResult> GetProductsInCart()
        {
            var userId = User.GetUserId();
            return Ok(await _productService.GetProductsInCart(userId));
        }

        [Authorize]
        [HttpPut("cart/{productId}/quantity/{quantity}")]
        public async Task<IActionResult> UpdateCartQuantity(int productId, int quantity)
        {
            var userId = User.GetUserId();
            var result = await _productService.UpdateCartQuantity(userId, productId, quantity);
            return result == "Cart quantity updated successfully" ? Ok(result) : BadRequest(result);
        }

        [Authorize]
        [HttpDelete("cart/delete/{productId}")]
        public async Task<IActionResult> DeleteProductInCart(int productId)
        {
            var userId = User.GetUserId();
            var result = await _productService.DeleteProductInCart(userId, productId);
            return result == "Product removed from cart successfully" ? Ok(result) : BadRequest(result);
        }

        [Authorize]
        [HttpDelete("cart/delete-all")]
        public async Task<IActionResult> DeleteAllProductsInCart()
        {
            var userId = User.GetUserId();
            var result = await _productService.DeleteAllProductsInCart(userId);
            return result == "All products removed from cart successfully" ? Ok(result) : BadRequest(result);
        }

        [Authorize]
        [HttpPost("product/{productId}/add-to-favorite")]
        public async Task<IActionResult> AddProductToFavorite(int productId)
        {
            var userId = User.GetUserId();
            var result = await _productService.AddProductToFavorite(userId, productId);
            return result == "Product added to Favorite successfully" ? Ok(result) : BadRequest(result);
        }

        [Authorize]
        [HttpDelete("product/{productId}/remove-from-favorite")]
        public async Task<IActionResult> RemoveProductFromFavorite(int productId)
        {
            var userId = User.GetUserId();
            var result = await _productService.DeleteProductFromFavorite(userId, productId);
            return result == "Product removed from Favorite successfully" ? Ok(result) : BadRequest(result);
        }

        [Authorize]
        [HttpGet("product/show-favoriteItems")]
        public async Task<IActionResult> GetProductFromFavorite()
        {
            var userId = User.GetUserId();
            return Ok(await _productService.GetProductFromFavorite(userId));
        }

        [HttpGet("products/search")]
        public async Task<IActionResult> SearchProducts([FromQuery] string? term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return BadRequest("Search term cannot be empty.");

            return Ok(await _productService.SearchProduct(term));
        }
    }
}