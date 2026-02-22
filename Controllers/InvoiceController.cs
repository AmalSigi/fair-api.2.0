using FairMount_api.Interfaces;
using FairMount_api.Models.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace FairMount_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
    private readonly IInvoiceRepository _repo;
    public InvoiceController(IInvoiceRepository repo)
    {
      _repo = repo;
    }

    #region Proforma invoice
    [HttpGet("proforma-invoices")]
    public async Task<IActionResult> GetProformaInvocies(DateOnly? startDate, DateOnly? endDate)
    {
      var invoices = await _repo.GetAllProformaInvoicesAsync(startDate, endDate);
      return Ok(invoices);
    }
    [HttpPost("proforma-invoices")]
    public async Task<IActionResult> CreateProformaInvoICe(ProformaInvoice proformaInvoice)
    {
      var createdInvoice =await _repo.CreateProformaInvoiceAsync(proformaInvoice);
      return Ok(createdInvoice);
    }

    [HttpGet("proforma-invoiceable-pos")]
    public async Task<IActionResult> GetProformaInvoiceablePOs()
    {
    var pos = await _repo.GetAllProformaInvoiceablePOsAsync();
     return Ok(pos);
     }
        [HttpGet("proforma/{id}")]
        public async Task<IActionResult> GetProformaById(int id)
        {
            var proforma = await _repo.GetProformaInvoiceByIdAsync(id);

            if (proforma == null)
            {
                return NotFound($"Proforma Invoice with ID {id} not found.");
            }

            return Ok(proforma);
        }


        [HttpPost("updateStatus")]
        public async Task<IActionResult> UpdateStatus(int id, int poId)
        {
            var updateStatus= await _repo.UpdateProformaInvoiceStatusAsync(id, poId);
            return Ok(updateStatus);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteProformaInvoiceAsync(int id,int poId)
        {
            var delete=await _repo.DeleteProformaInvoiceAsync(id, poId);
            if (delete)
            {
                return NoContent();
            }
            else {

                return NotFound();
            }
        }

        #endregion
    }
}
