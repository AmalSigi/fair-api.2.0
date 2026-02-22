using FairMount_api.Data;
using FairMount_api.Interfaces;
using FairMount_api.Models.Dtos;
using FairMount_api.Models.Tables;
using Microsoft.EntityFrameworkCore;

namespace FairMount_api.Repository
{
    public class SLIRepository : ISLIRepository
    {
        private readonly FairmountDbContext _context;

        public SLIRepository(FairmountDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Sli_Document>> GetAllAsync()
        {
            return await _context.SliDocuments
                .AsNoTracking() 
                .Include(x => x.Items)
                .ToListAsync();
        }
        public async Task<Sli_Document?> GetByIdAsync(int id)
        {
            return await _context.SliDocuments
                .AsNoTracking() 
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.SliId == id);
        }
        // SLIRepository.cs
        public async Task<Sli_Document> CreateAsync(Sli_Document sli)
        {

            await _context.SliDocuments.AddAsync(sli);
            await _context.SaveChangesAsync();
            return sli;
        }
        public async Task<Sli_Document?> GetByComercialIdAsync(int id)
        {
            return await _context.SliDocuments
                .AsNoTracking()
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.CommercialInvoiceId == id);
        }
    }
}