using FairMount_api.Models.Tables;

namespace FairMount_api.Interfaces
{
  public interface IOrganizationRepository
  {
    Task<List<Organizations>> GetAllOrgs();
    Task<Organizations?> GetOrgByIdAsync(int orgId);
    Task<Organizations?> AddOrganizationAsync(Organizations org);
    Task<bool> UpdateOrganizationAsync(Organizations org);
    Task<bool> DeleteOrganizationAsync(int orgId);

    Task<List<OrganizationAddresses>> GetAddressesByOrgIdAsync(int orgId);
    Task<OrganizationAddresses?> AddAddressAsync(OrganizationAddresses address);
    Task<bool> UpdateAddressAsync(OrganizationAddresses address);
    Task<bool> DeleteAddressAsync(int addressId);
  }
}
