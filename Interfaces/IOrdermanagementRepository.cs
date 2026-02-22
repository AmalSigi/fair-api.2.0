using FairMount_api.Models.Dtos;

namespace FairMount_api.Interfaces
{
    public interface IOrdermanagementRepository
    {
        Task<PurchaseOrders> CreatePO(PurchaseOrders order);
        Task<List<PoDto>> GetActivePOS();
        Task<List<POItem>> GetPOItems(int poId);
        Task<List<Customer>> GetCustomers();
    Task<int> AddPOItem(POItem item);
    Task<int> UpdatePOItem(POItem item);
    Task<int> BulkAddPOItems(List<POItem> items,int poId,int? typeId,string? typeName);
    Task<List<ImportPOResponseDto>> BulkImportPOS (List<ImportPurchaseOrderDto> pos);
        Task<bool> PoStatusUpdate(int statusId,int poId);
  }
}
