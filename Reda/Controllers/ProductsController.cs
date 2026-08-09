using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reda.Data;
using Reda.Dtos;
using Reda.Interfaces;
using Reda.Services;
using System.Threading.Tasks;

namespace Reda.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _categoryService;

        public ProductsController(IProductService categoryService)
        {
            _categoryService = categoryService;
        }
        
        [HttpGet("products/category/{category}")]
        public async Task<IActionResult> GetProductByCategory(string category)
        {
            var result = await _categoryService.GetProductsByCategoryAsync(category);
            if (result != null) return Ok(result);
            return NotFound();
        }
        
        [HttpGet("product/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var result = await _categoryService.GetProductByIdAsync(id);
            if (result != null) return Ok(result);
            return NotFound();
        }

        [Authorize]
        [HttpPost("product/{productId}/add-to-cart")]
        public async Task<IActionResult> AddProductToCart(int productId,[FromBody] UserIdDto model)
        {
            var result = await _categoryService.AddProductToCart(productId, model.UserId);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("cart")]
        public async Task<IActionResult> GetProductsInCart([FromQuery] UserIdDto model)
        {
            var result = await _categoryService.GetProductsInCart(model.UserId);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("cart/delete/{ProductId}")]
        public async Task<IActionResult> DeleteProductInCart([FromBody] UserIdDto model,int ProductId)
        {
            var result = await _categoryService.DeleteProductInCart(model.UserId, ProductId);
            if(result != null) return Ok(result);
            return NotFound();
        }

        [Authorize]
        [HttpDelete("cart/delete-all")]
        public async Task<IActionResult> DeleteAllProductsInCart([FromBody] UserIdDto model) 
        {
            var result = await _categoryService.DeleteAllProductsInCart(model.UserId);
            if (result != null) return Ok();
            return NotFound();
        }

        [Authorize]
        [HttpPost("product/{productId}/add-to-favorite")]
        public async Task<IActionResult> AddProductToFavorite([FromBody] UserIdDto model, int productId)
        {
            try
            {
                var result = await _categoryService.AddProductToFavorite(model.UserId, productId);
                if (result != null) return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred.",
                    error = ex.Message

                });
            }
            return BadRequest();
        }

        [Authorize]
        [HttpDelete("product/{productId}/remove-from-favorite")]
        public async Task<IActionResult> RemoveProductFromFavorite([FromBody] UserIdDto model, int productId)
        {
            var result = await _categoryService.DeleteProductFromFavorite(model.UserId, productId);
            if(result != null)
            {
                return Ok(result);
            }
            return BadRequest();
        }

        [Authorize]
        [HttpGet("product/show-favoriteItems")]
        public async Task<IActionResult> GetProductFromFavorite([FromQuery] UserIdDto model)
        {
            var result = await _categoryService.GetProductFromFavorite(model.UserId);
            if(result != null) return Ok(result);
            return NotFound();
        }

        [HttpGet("products/search")]
        public async Task<IActionResult> SearchProducts([FromQuery] string? term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {

                return Ok("Search term cannot be empty.");
            }

            var products = await _categoryService.SearchProduct(term);
            return Ok(products);
        }
    }
}