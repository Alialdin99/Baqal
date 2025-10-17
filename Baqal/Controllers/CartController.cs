using Baqal.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Baqal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        // Helper: Get userId or session key
        private (string Key, bool IsAuthenticated) GetCartContext()
        {
            // Ensure session exists
            HttpContext.Session.SetString("Init", "1");

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userId))
                    return (userId, true);
            }

            // Guest cart
            return (HttpContext.Session.Id, false);
        }

        //  Add item to cart
        [HttpPost("add")]
        public async Task<IActionResult> AddItem([FromQuery] string productId, [FromQuery] int quantity = 1)
        {
            if (string.IsNullOrEmpty(productId))
                return BadRequest(new { message = "ProductId is required." });

            if (quantity <= 0)
                return BadRequest(new { message = "Quantity must be greater than zero." });

            var (cartKey, isAuthenticated) = GetCartContext();

            if (isAuthenticated)
                await _cartService.AddItemToUserCartAsync(cartKey, productId, quantity);
            else
                await _cartService.AddItemAsync(cartKey, productId, quantity);

            return Ok(new { message = "Item added to cart successfully." });
        }

        //  Get current cart
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentCart()
        {
            var (cartKey, isAuthenticated) = GetCartContext();

            if (isAuthenticated)
            {
                var cart = await _cartService.GetCartByUserIdAsync(cartKey);
                if (cart == null)
                    return NotFound(new { message = "Cart not found." });
                return Ok(cart);
            }
            else
            {
                var cart = await _cartService.GetCartBySessionIdAsync(cartKey);
                return Ok(cart); // session-based cart always returns a list (never null)
            }
        }

        //  Update quantity
        [HttpPut("update")]
        public async Task<IActionResult> UpdateItem([FromQuery] string productId, [FromQuery] int quantity)
        {
            if (quantity <= 0)
                return BadRequest(new { message = "Quantity must be greater than zero." });

            var (cartKey, isAuthenticated) = GetCartContext();

            if (isAuthenticated)
                await _cartService.UpdateUserCartQuantityAsync(cartKey, productId, quantity);
            else
                await _cartService.UpdateQuantityAsync(cartKey, productId, quantity);

            return Ok(new { message = "Cart item updated successfully." });
        }

        //  Remove item
        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveItem([FromQuery] string productId)
        {
            if (string.IsNullOrEmpty(productId))
                return BadRequest(new { message = "ProductId is required." });

            var (cartKey, isAuthenticated) = GetCartContext();

            if (isAuthenticated)
                await _cartService.RemoveItemFromUserCartAsync(cartKey, productId);
            else
                await _cartService.RemoveItemAsync(cartKey, productId);

            return Ok(new { message = "Item removed from cart." });
        }

        // Clear cart
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var (cartKey, isAuthenticated) = GetCartContext();

            if (isAuthenticated)
                await _cartService.ClearUserCartAsync(cartKey);
            else
                await _cartService.ClearCartAsync(cartKey);

            return Ok(new { message = "Cart cleared successfully." });
        }
    }
}
