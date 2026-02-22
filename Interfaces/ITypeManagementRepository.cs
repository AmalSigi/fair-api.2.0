using FairMount_api.Models.Tables;

namespace FairMount_api.Interfaces
{
  public interface ITypeManagementRepository
  {
    Task<List<PoStatus>> GetAllPOTypesAsync();
    Task<List<POitemStatus>> GetAllItemStatus();
  }
}
