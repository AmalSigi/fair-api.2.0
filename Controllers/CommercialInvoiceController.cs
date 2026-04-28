using FairMount_api.Application.Interfaces;
using FairMount_api.Models.Tables;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FairMount_api.Api.Controllers
{
    [ApiController]
    [Route("api/CommercialInvoice")]
    public class CommercialInvoiceController : ControllerBase
    {
        private readonly ICommercialInvoiceRepository _repo;

        public CommercialInvoiceController(ICommercialInvoiceRepository repo)
        {
            _repo = repo;
        }

        // GET: api/commercialinvoice/eligible-pos
        [HttpGet("eligible-pos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEligiblePurchaseOrders()
        {
            var pos = await _repo.GetEligiblePurchaseOrdersAsync();
            return Ok(pos);
        }

        // GET: api/commercialinvoice
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllInvoices(DateOnly? startDate, DateOnly? endDate)
        {
            var invoices = await _repo.GetAllInvoicesAsync(startDate, endDate);
            return Ok(invoices);
        }

        // GET: api/commercialinvoice/{invoiceNumber}
        [HttpGet("{invoiceId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetInvoice(int invoiceId)
        {
            var invoice = await _repo.GetInvoiceAsync(invoiceId);
            if (invoice == null)
            {
                return NotFound($"Invoice with number {invoiceId} not found.");
            }
            return Ok(invoice);
        }

        // POST: api/commercialinvoice
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateInvoice([FromBody] CommercialInvoice invoice)
        {
            if (await _repo.InvoiceExistsAsync(invoice.CommercialInvoiceNumber))
            {
                return Conflict($"Invoice number {invoice.CommercialInvoiceNumber} already exists.");
            }

            if (invoice.CreatedAt == default)
            {
                invoice.CreatedAt = System.DateTime.Now.Date;
            }

            await _repo.CreateInvoiceAsync(invoice);

            return CreatedAtAction(nameof(GetInvoice), new { invoiceId = invoice.CommercialInvoiceId }, invoice);
        }

        // PUT: api/commercialinvoice/{invoiceNumber}
        [HttpPut("{invoiceNumber}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateInvoice(string invoiceNumber, [FromBody] CommercialInvoice invoice)
        {
            // Ensure the route parameter matches the body payload's unique identifier.
            if (invoiceNumber != invoice.CommercialInvoiceNumber)
            {
                return BadRequest("Invoice Number in URL does not match body payload.");
            }

            if (!await _repo.InvoiceExistsAsync(invoiceNumber))
            {
                return NotFound($"Invoice with number {invoiceNumber} not found for update.");
            }

            await _repo.UpdateInvoiceAsync(invoice);
            return NoContent();
        }

        // GET: api/commercialinvoice
        [HttpGet("invoiceTax")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTInvoicesTax(DateTime? startDate, DateTime? endDate)
        {
            var invoices = await _repo.GetInvoicesTaxAsync(startDate, endDate);
            return Ok(invoices);
        }

        //[HttpPost]
        //public async Task<IActionResult> DeleteComericalInvoiceAsync(int id, int poId) {
        //    var delete = await _repo.DeleteComericalInvoiceAsync(id, poId);
        //    if (delete)
        //    {
        //        return NoContent();
        //    }
        //    else
        //    {

        //        return NotFound();
        //    }
        //}
    }
}