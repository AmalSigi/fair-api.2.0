using FairMount_api.Models.Tables;

namespace FairMount_api.Interfaces
{
  public interface IInvoiceRepository
  {
    Task<List<ProformaInvoice>> GetAllProformaInvoicesAsync(DateOnly? startDate,DateOnly? endDate);
    Task<ProformaInvoice> CreateProformaInvoiceAsync(ProformaInvoice proformaInvoice);
        Task<List<PurchaseOrders>> GetAllProformaInvoiceablePOsAsync();
        Task<ProformaInvoice?> GetProformaInvoiceByIdAsync(int proformaId);
        Task<ProformaInvoice> UpdateProformaInvoiceStatusAsync(int id , int poId);
        Task<bool> DeleteProformaInvoiceAsync(int id, int poId);
    }
}
