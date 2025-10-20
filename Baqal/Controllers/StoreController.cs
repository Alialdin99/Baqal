using Baqal.Application.DTOs;
using Baqal.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Baqal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly StoreService _storeService;

        public StoreController(StoreService storeService)
        {
            _storeService = storeService;
        }

        [HttpPost("AddStore")]
        public async Task<IActionResult> AddStore([FromBody] AddStoreDTO addStoreDTO)
        {
            if (addStoreDTO == null)
            {
                return BadRequest("Request Body is null.");
            }

            var result = await _storeService.AddAsync(addStoreDTO);
            return Ok(result);
        }

        [HttpGet("GetStore/{id}")]
        public async Task<IActionResult> GetStoreAsync([FromQuery] Guid id)
        {
            var store = await _storeService.GetByIdAsync(id);

            if (store == null)
            {
                return NotFound("Store Not Found.");
            }

            return Ok(store);
        }

        [HttpGet("GetAllStores")]
        public async Task<IActionResult> GetAllStores()
        {
            var stores = await _storeService.GetAllAsync();
            return Ok(stores);
        }

        [HttpPut("UpdateStore")]
        public async Task<IActionResult> UpdateStore(
            [FromQuery] Guid id,
            [FromBody] UpdateStoreDTO updateStoreDTO)
        {
            var result = await _storeService.UpdateAsync(id, updateStoreDTO);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpDelete("DeleteStore")]
        public async Task<IActionResult> DeleteStore([FromQuery] Guid id)
        {
            var result = await _storeService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return Ok("Store Deleted");
        }
    }
}
