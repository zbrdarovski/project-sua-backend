using InventoryAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace InventoryAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InventoryController : ControllerBase
    {

        private readonly InventoryRepository _inventoryRepository;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(ILogger<InventoryController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _inventoryRepository = new InventoryRepository(configuration);
        }

        [HttpGet(Name = "GetAllItems")]
        public async Task<IEnumerable<Inventory>> GetAllItems()
        {
           var items = await _inventoryRepository.GetAllItemsAsync();
           return items;
        }

        [HttpGet("{itemId}", Name = "GetItemById")]
        public async Task<ActionResult<Inventory>> GetItemById(string itemId)
        {
            var item = await _inventoryRepository.GetItemByIdAsync(itemId);

            if (item == null)
            {
                return NotFound("Item not found");
            }

            return item;
        }

        [HttpPost(Name = "AddItem")]
        public async Task<IActionResult> AddItem([FromBody] Inventory item)
        {
            await _inventoryRepository.AddItemAsync(item);
            return CreatedAtRoute("GetItemById", new { itemId = item.Id.ToString() }, item);
        }

        [HttpPut("{itemId}", Name = "UpdateItem")]
        public async Task<IActionResult> UpdateItem(string itemId, [FromBody] Inventory updatedItem)
        {
            var existingItem = await _inventoryRepository.GetItemByIdAsync(itemId);

            if (existingItem == null)
            {
                return NotFound("Item not found");
            }

            updatedItem.Id = existingItem.Id;
            await _inventoryRepository.UpdateItemAsync(updatedItem);

            return Ok(updatedItem);
        }

        [HttpDelete("{itemId}", Name = "DeleteItem")]
        public async Task<IActionResult> DeleteItem(string itemId)
        {
            var existingItem = await _inventoryRepository.GetItemByIdAsync(itemId);

            if (existingItem == null)
            {
                return NotFound("Item not found");
            }

            await _inventoryRepository.DeleteItemAsync(itemId);

            return NoContent();
        }

        [HttpPost("{itemId}/addquantity/{quantityToAdd}", Name = "AddItemQunatityById")]
        public async Task<IActionResult> AddQuantity(string itemId, int quantityToAdd)
        {
            try
            {
                await _inventoryRepository.AddQuantityAsync(itemId, quantityToAdd);
                return Ok($"Added {quantityToAdd} to quantity of item {itemId}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{itemId}/subtractquantity/{quantityToSubtract}", Name = "SubtractItemQuantityById")]
        public async Task<IActionResult> SubtractQuantity(string itemId, int quantityToSubtract)
        {
            try
            {
                await _inventoryRepository.SubtractQuantityAsync(itemId, quantityToSubtract);
                return Ok($"Subtracted {quantityToSubtract} from quantity of item {itemId}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{itemId}/changeprice/{newPrice}", Name = "ChangeItemPriceById")]
        public async Task<IActionResult> ChangePrice(string itemId, double newPrice)
        {
            try
            {
                await _inventoryRepository.ChangePriceAsync(itemId, newPrice);
                return Ok($"Changed price of item {itemId} to {newPrice}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{itemId}/comments", Name = "GetComments")]
        public async Task<ActionResult<List<Comment>>> GetComments(string itemId)
        {
            var comments = await _inventoryRepository.GetCommentsAsync(itemId);
            return comments;
        }

        [HttpGet("{itemId}/ratings", Name = "GetRatings")]
        public async Task<ActionResult<List<Rating>>> GetRatings(string itemId)
        {
            var ratings = await _inventoryRepository.GetRatingsAsync(itemId);
            return ratings;
        }

    }
}