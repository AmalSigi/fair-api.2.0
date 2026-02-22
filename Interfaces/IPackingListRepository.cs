using FairMount_api.Models.Tables;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FairMount_api.Application.Interfaces
{
    public interface IPackingListRepository
    {
        Task<bool> PackingListExistsAsync(int packingListId);

        Task<PackingList?> GetPackingListAsync(int packingListId);
        Task<PackingList?> GetPackingListByCommericalInvoiceIdAsync(int orderId);
        Task<IEnumerable<PackingList>> GetAllPackingListsAsync();
        Task<int> CreatePackingListAsync(PackingList packingList);
        Task UpdatePackingListAsync(PackingList packingList);
    }
}