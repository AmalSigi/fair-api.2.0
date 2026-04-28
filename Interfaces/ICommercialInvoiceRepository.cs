using FairMount_api.Models.Dtos;
using FairMount_api.Models.Tables;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FairMount_api.Application.Interfaces
{
    public interface ICommercialInvoiceRepository
    {
        // Signature uses the string invoice number for lookup
        Task<bool> InvoiceExistsAsync(string invoiceNumber);

        // CRUD Operations
        Task<CommercialInvoice?> GetInvoiceAsync(int invoiceId);
        Task<List<CommercialInvoice>> GetAllInvoicesAsync(DateOnly? startDate, DateOnly? endDate);
        Task<CommercialInvoice> CreateInvoiceAsync(CommercialInvoice invoice);
        Task UpdateInvoiceAsync(CommercialInvoice invoice);

        // Business Logic
        Task<IEnumerable<object>> GetEligiblePurchaseOrdersAsync();
        Task<bool> DeleteComericalInvoiceAsync(int id, int poId);
        Task<List<InvoiceTax>> GetInvoicesTaxAsync(DateTime? startDate, DateTime? endDate);
    }
}