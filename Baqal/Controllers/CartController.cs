using Baqal.Application.DTOs;
using Baqal.Application.DTOs.CartDTOs;
using Baqal.Application.DTOs.common;
using Baqal.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Baqal.API.Controllers
{
    /// Manages shopping cart operations for both guest and authenticated users
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
        }

        #region GET Endpoints

        /// Get current user's shopping cart
        /// Works for both authenticated users and guests (using session cookies)
        /// <response code="200">Returns the current cart</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [ProducesResponseType(typeof(CartResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CartResponseDto>> GetCart()
        {
            try
            {
                var cart = await _cartService.GetCurrentCartAsync();
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse
                {
                    Message = "An error occurred while retrieving your cart",
                    Details = ex.Message
                });
            }
        }

        /// Get total number of items in cart
        /// Useful for displaying cart badge count in navigation
        /// <response code="200">Returns the item count</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("count")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> GetCartItemCount()
        {
            try
            {
                var count = await _cartService.GetCartItemCountAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse
                {
                    Message = "An error occurred while retrieving cart count",
                    Details = ex.Message
                });
            }
        }

        /// Get total price of all items in cart
        /// <response code="200">Returns the cart total</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("total")]
        [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<decimal>> GetCartTotal()
        {
            try
            {
                var total = await _cartService.GetCartTotalAsync();
                return Ok(new { total });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse
                {
                    Message = "An error occurred while calculating cart total",
                    Details = ex.Message
                });
            }
        }

        #endregion

        #region POST Endpoints

        /// Add item to cart
        /// <param name="request">Product ID and quantity to add</param>
        /// <response code="200">Item added successfully</response>
        /// <response code="400">Invalid request (product not found, insufficient stock, etc.)</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("items")]
        [ProducesResponseType(typeof(CartResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CartResponseDto>> AddItem([FromBody] CartItemDto request)
        {
            try
            {
                // Model validation is automatic with [ApiController] attribute
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Invalid request data",
                        Details = string.Join(", ", ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage))
                    });
                }

                var cart = await _cartService.AddItemAsync(request);
                return Ok(cart);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorResponse
                {
                    Message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErrorResponse
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse
                {
                    Message = "An error occurred while adding item to cart",
                    Details = ex.Message
                });
            }
        }

        /// Merge guest cart with user cart after login
        /// <response code="200">Carts merged successfully</response>
        /// <response code="401">User not authenticated</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("merge")]
        [Authorize] // Requires authentication
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> MergeGuestCart()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ErrorResponse
                    {
                        Message = "User not authenticated"
                    });
                }

                await _cartService.MergeGuestCartOnLoginAsync(userId);
                return Ok(new { message = "Cart merged successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse
                {
                    Message = "An error occurred while merging carts",
                    Details = ex.Message
                });
            }
        }

        #endregion

        #region PUT Endpoints

        /// Update quantity of item in cart
        /// <param name="request">Product ID and new quantity</param>
        /// <response code="200">Item updated successfully</response>
        /// <response code="400">Invalid request</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("items")]
        [ProducesResponseType(typeof(CartResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CartResponseDto>> UpdateItem([FromBody] CartItemDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Invalid request data",
                        Details = string.Join(", ", ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage))
                    });
                }

                var cart = await _cartService.UpdateItemAsync(request);
                return Ok(cart);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorResponse
                {
                    Message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErrorResponse
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse
                {
                    Message = "An error occurred while updating cart item",
                    Details = ex.Message
                });
            }
        }

        #endregion

        #region DELETE Endpoints

        /// Remove item from cart
        /// <param name="productId">ID of the product to remove</param>
        /// <response code="200">Item removed successfully</response>
        /// <response code="400">Invalid product ID</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("items/{productId}")]
        [ProducesResponseType(typeof(CartResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CartResponseDto>> RemoveItem([FromRoute] Guid productId)
        {
            try
            {
                if (productId == Guid.Empty)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Invalid product ID"
                    });
                }

                var cart = await _cartService.RemoveItemAsync(productId);
                return Ok(cart);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorResponse
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse
                {
                    Message = "An error occurred while removing item from cart",
                    Details = ex.Message
                });
            }
        }

        /// Clear all items from cart
        /// <response code="204">Cart cleared successfully</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                await _cartService.ClearCartAsync();
                return NoContent(); // 204 - Success with no content to return
            }
            catch (Exception exp)
            {
                return StatusCode(500, new ErrorResponse
                {
                    Message = "An error occurred while clearing cart",
                    Details = exp.Message
                });
            }
        }

        #endregion
    }
}