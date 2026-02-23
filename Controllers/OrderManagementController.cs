using FairMount_api.Interfaces;
using FairMount_api.Models.Dtos;
using FairMount_api.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FairMount_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class OrderManagementController : ControllerBase
    {
        private readonly IOrdermanagementRepository _orderManagementRepository;
        public OrderManagementController(IOrdermanagementRepository orderManagementRepository)
        {
            _orderManagementRepository = orderManagementRepository;
        }
        [HttpPost("CreatePO")]
        public async Task<IActionResult> CreatePO([FromBody] PurchaseOrders order)
        {
            if (order == null)
            {
                return BadRequest("Purchase order cannot be null");
            }
            try
            {
                var poId = await _orderManagementRepository.CreatePO(order);
                return Ok(poId);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An internal error occurred." });
            }
        }
        //bulk import pos with items
        [HttpPost("import")]
        public async Task<IActionResult> ImportPOs([FromBody] List<ImportPurchaseOrderDto> orders)
        {
            if (orders == null || !orders.Any())
            {
                return BadRequest("Purchase orders cannot be null or empty");
            }
            try
            {
                var result = await _orderManagementRepository.BulkImportPOS(orders);
                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("GetActivePOS")]
        public async Task<IActionResult> GetActivePOS()
        {
            try
            {
                var activePOS = await _orderManagementRepository.GetActivePOS();
                if (activePOS == null || !activePOS.Any())
                {
                    return NotFound("No active purchase orders found.");
                }
                return Ok(activePOS);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("GetPOItems/{poId}")]
        public async Task<IActionResult> GetPOItems(int poId)
        {
            try
            {
                var poItems = await _orderManagementRepository.GetPOItems(poId);
                if (poItems == null || !poItems.Any())
                {
                    return NotFound($"No items found for purchase order ID {poId}.");
                }
                return Ok(poItems);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost("AddPOItem")]
        public async Task<IActionResult> AddPOItem([FromBody] POItem item)
        {
            if (item == null)
            {
                return BadRequest("PO Item cannot be null");
            }
            try
            {
                await _orderManagementRepository.AddPOItem(item);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost ("UpdatePOItem")]
        public async Task<IActionResult> UpdatePOItem([FromBody] POItem item)
        {
            if (item == null)
            {
                return BadRequest("PO Item cannot be null");
            }
            try
            {
                var result = await _orderManagementRepository.UpdatePOItem(item);
                if (result == 1)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("BulkAdd")]
        public async Task<IActionResult> BulkAddItems([FromBody] List<POItem> items, [FromQuery] int poId, [FromQuery] int? typeId, [FromQuery] string? typeName)
        {
            var result = await _orderManagementRepository.BulkAddPOItems(items, poId, typeId, typeName);
            if (result == 1)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        //customers
        [HttpGet("GetCustomers")]
        public async Task<IActionResult> GetCustomers()
        {
            try
            {
                var customers = await _orderManagementRepository.GetCustomers();
                if (customers == null || !customers.Any())
                {
                    return NotFound("No customer found.");
                }
                return Ok(customers);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost("UpdatePoStatus")]
        public async Task<IActionResult> PoStatusUpdate(int statusId, int poId)
        {
            try
            {
                var updated = await _orderManagementRepository.PoStatusUpdate(statusId, poId);
                if (!updated) return NotFound();
                return NoContent();

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPost("UpdatePo")]
        public async Task<IActionResult> POUpdate(int poId, [FromBody] PurchaseOrders po)
        {
            try
            {
                var updated = await _orderManagementRepository.PoUpdate(poId, po);
                if (updated == null) return NotFound();
                return NoContent();

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }

        }

    }
    
}
