using FairMount_api.Data;
using FairMount_api.Interfaces;
using FairMount_api.Models.Tables;
using Microsoft.EntityFrameworkCore;

namespace FairMount_api.Repository
{
  public class OrganizationRepository : IOrganizationRepository
  {
    private readonly FairmountDbContext _dbContext;
    public OrganizationRepository(FairmountDbContext context) {
      _dbContext = context;
    }
        /// <summary>
        /// Get all organizations along with their addresses
        /// </summary>
        public async Task<List<Organizations>> GetAllOrgs()
        {
            try
            {
                return await _dbContext.Organizations
                    .AsNoTracking() // Disables change tracking for better performance
                    .Include(o => o.Addresses)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<Organizations>();
            }
        }

        /// <summary>
        /// Get organization by ID with its addresses
        /// </summary>
        public async Task<Organizations?> GetOrgByIdAsync(int orgId)
        {
            return await _dbContext.Organizations
                .AsNoTracking() // Essential for high-performance read-only queries
                .Include(o => o.Addresses)
                .FirstOrDefaultAsync(o => o.OrganizationId == orgId);
        }
        /// <summary>
        /// Add a new organization
        /// </summary>
        public async Task<Organizations?> AddOrganizationAsync(Organizations org)
    {
      try
      {
        _dbContext.Organizations.Add(org);
        await _dbContext.SaveChangesAsync();
        return org;
      }
      catch (Exception)
      {
        return null;
      }
    }

    /// <summary>
    /// Update existing organization details
    /// </summary>
    public async Task<bool> UpdateOrganizationAsync(Organizations org)
    {
      var existing = await _dbContext.Organizations.FindAsync(org.OrganizationId);
      if (existing == null) return false;

      existing.Name = org.Name;
      existing.Type = org.Type;
      existing.Email = org.Email;
      existing.Phone = org.Phone;
      existing.TaxId = org.TaxId;
      existing.IsActive = org.IsActive;

      _dbContext.Organizations.Update(existing);
      await _dbContext.SaveChangesAsync();
      return true;
    }

    /// <summary>
    /// Delete organization and its addresses (cascade)
    /// </summary>
    public async Task<bool> DeleteOrganizationAsync(int orgId)
    {
      var org = await _dbContext.Organizations
          .Include(o => o.Addresses)
          .FirstOrDefaultAsync(o => o.OrganizationId == orgId);

      if (org == null) return false;

      _dbContext.Organizations.Remove(org);
      await _dbContext.SaveChangesAsync();
      return true;
    }

        // -----------------------------
        // Address Operations
        // -----------------------------

        /// <summary>
        /// Get all addresses for a given organization
        /// </summary>
        public async Task<List<OrganizationAddresses>> GetAddressesByOrgIdAsync(int orgId)
        {
            return await _dbContext.OrganizationAddresses
                .AsNoTracking()
                .Where(a => a.OrganizationId == orgId)
                .ToListAsync();
        }

        /// <summary>
        /// Add a new address for an organization
        /// </summary>
        public async Task<OrganizationAddresses?> AddAddressAsync(OrganizationAddresses address)
    {
      try
      {
        _dbContext.OrganizationAddresses.Add(address);
        await _dbContext.SaveChangesAsync();
        return address;
      }
      catch (Exception)
      {
        return null;
      }
    }

    /// <summary>
    /// Update an address
    /// </summary>
    public async Task<bool> UpdateAddressAsync(OrganizationAddresses address)
    {
      var existing = await _dbContext.OrganizationAddresses.FindAsync(address.AddressId);
      if (existing == null) return false;

      existing.AddressType = address.AddressType;
      existing.AddressLine1 = address.AddressLine1;
      existing.AddressLine2 = address.AddressLine2;
      existing.City = address.City;
      existing.State = address.State;
      existing.PostalCode = address.PostalCode;
      existing.Country = address.Country;

      _dbContext.OrganizationAddresses.Update(existing);
      await _dbContext.SaveChangesAsync();
      return true;
    }

    /// <summary>
    /// Delete an address
    /// </summary>
    public async Task<bool> DeleteAddressAsync(int addressId)
    {
      var address = await _dbContext.OrganizationAddresses.FindAsync(addressId);
      if (address == null) return false;

      _dbContext.OrganizationAddresses.Remove(address);
      await _dbContext.SaveChangesAsync();
      return true;
    }
  }
}
