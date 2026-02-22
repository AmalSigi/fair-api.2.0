using FairMount_api.Data;
using FairMount_api.Interfaces;
using FairMount_api.Models.Tables;
using Microsoft.EntityFrameworkCore;

namespace FairMount_api.Repository
{
  public class TypeManagementRepository : ITypeManagementRepository
  {
    private readonly FairmountDbContext _dbContext;
    public TypeManagementRepository(FairmountDbContext context) {
      _dbContext = context;
    }

        #region po type management
        //Get all types available for pos
        public async Task<List<PoStatus>> GetAllPOTypesAsync()
        {
            return await _dbContext.POStatus
                .AsNoTracking()
                .ToListAsync();
        }

        #endregion
        #region po item status management
        public async Task<List<POitemStatus>> GetAllItemStatus()
        {
            return await _dbContext.POItemStatus
                .AsNoTracking()
                .ToListAsync();
        }

        #endregion
    }
}
