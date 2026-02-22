using FairMount_api.Application.Interfaces;
using FairMount_api.Models.Tables;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FairMount_api.Api.Controllers
{
    [ApiController]
    [Route("api/PackingList")]
    public class PackingListController : ControllerBase
    {
        private readonly IPackingListRepository _repo;

        public PackingListController(IPackingListRepository repo)
        {
            _repo = repo;
        }

        // GET: api/packinglist
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllPackingLists()
        {
            var packingLists = await _repo.GetAllPackingListsAsync();
            return Ok(packingLists);
        }

        // GET: api/packinglist/{packingListId}
        [HttpGet("{packingListId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPackingList(int packingListId)
        {
            var packingList = await _repo.GetPackingListAsync(packingListId);
            if (packingList == null)
            {
                return NotFound($"Packing List with ID {packingListId} not found.");
            }
            return Ok(packingList);
        }

        // GET: api/packinglist/{commercialInvoiceId}
        [HttpGet("Commercial/{commercialInvoiceId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPackingListByCommercialInvoice(int commercialInvoiceId)
        {
            var packingLists = await _repo.GetPackingListByCommericalInvoiceIdAsync(commercialInvoiceId);
            if (packingLists==null)
            {
                return NotFound($"No Packing Lists found for Commercial Invoice ID {commercialInvoiceId}.");
            }
            return Ok(packingLists);
        }

        // POST: api/packinglist
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatePackingList([FromBody] PackingList packingList)
        {
            if (packingList.CreatedAt == default)
            {
                packingList.CreatedAt = System.DateTime.Now.Date;
            }

            int newId = await _repo.CreatePackingListAsync(packingList);

            packingList.PackingListId = newId;

            return CreatedAtAction(nameof(GetPackingList), new { packingListId = newId }, packingList);
        }

        // PUT: api/packinglist/{packingListId}
        [HttpPut("{packingListId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePackingList(int packingListId, [FromBody] PackingList packingList)
        {
            // Validate that the route ID matches the body payload ID
            if (packingListId != packingList.PackingListId)
            {
                return BadRequest("Packing List ID in URL does not match body payload.");
            }

            if (!await _repo.PackingListExistsAsync(packingListId))
            {
                return NotFound($"Packing List with ID {packingListId} not found for update.");
            }

            await _repo.UpdatePackingListAsync(packingList);
            return NoContent();
        }
    }


}