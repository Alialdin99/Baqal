using Baqal.Application.DTOs;
using Baqal.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Baqal.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct(AddProductDTO addProductDTO)
        {
            if (addProductDTO == null)
            {
                return BadRequest("Request Body is null.");
            }

            var result = await _productService.AddAsync(addProductDTO);
            return Ok(result);

        }

        [HttpGet("GetProduct/{id}")]
        public async Task<IActionResult> GetProductAsync(Guid id)
        {
            var product = await _productService.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound("Product Not Found.");
            }
            return Ok(product);
        }

        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GettAllProducts()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        [HttpPut("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct(Guid id, UpdateProductDTO updateProductDTO)
        {
            var result = await _productService.UpdateAsync(id, updateProductDTO);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpDelete("DeleteProduct")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var result = await _productService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return Ok("Product Deleted");
        }
    }
}