using FairMount_api.Application.Interfaces;
using FairMount_api.Data; // Assumed DbContext location
using FairMount_api.Models.Tables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FairMount_api.Repository
{
    public class PackingListRepository : IPackingListRepository
    {
        private readonly FairmountDbContext _context;

        public PackingListRepository(FairmountDbContext context)
        {
            _context = context;
        }

        public async Task<bool> PackingListExistsAsync(int packingListId)
        {
            return await _context.PackingList.AnyAsync(pl => pl.PackingListId == packingListId);
        }

     public async Task<PackingList?> GetPackingListAsync(int packingListId)
{
    return await _context.PackingList
        .AsNoTracking()
        .Where(pl => pl.PackingListId == packingListId)
        .Select(pl => new PackingList
        {
            PackingListId = pl.PackingListId,
            CommercialInvoiceId = pl.CommercialInvoiceId,
            CreatedAt = pl.CreatedAt,
            PackingListItems = pl.PackingListItems.ToList()
        })
        .FirstOrDefaultAsync();
}
        public async Task<IEnumerable<PackingList>> GetAllPackingListsAsync()
        {
            return await _context.PackingList
                .AsNoTracking()
                .OrderByDescending(pl => pl.CreatedAt)
                .Select(pl => new PackingList
                {
                    PackingListId = pl.PackingListId,
                    CommercialInvoiceId = pl.CommercialInvoiceId,
                    CreatedAt = pl.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<PackingList?> GetPackingListByCommericalInvoiceIdAsync(int commercialInvoiceId)
        {
            return await _context.PackingList
                .AsNoTracking()
                .Where(pl => pl.CommercialInvoiceId == commercialInvoiceId)
                .Select(pl => new PackingList
                {
                    PackingListId = pl.PackingListId,
                    CommercialInvoiceId = pl.CommercialInvoiceId,
                    CreatedAt = pl.CreatedAt,
                    PackingListItems = pl.PackingListItems.ToList()
                })
                .FirstOrDefaultAsync();
        }
        public async Task<int> CreatePackingListAsync(PackingList packingList)
        {
            _context.PackingList.Add(packingList);

            // This saves the data to the database
            await _context.SaveChangesAsync();

            // EF Core automatically populates the 'Id' property on the 
            // object after SaveChangesAsync is successful.
            return packingList.PackingListId;
        }

        public async Task UpdatePackingListAsync(PackingList packingList)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingPackingList = await _context.PackingList
                    .Include(pl => pl.PackingListItems)
                    .FirstOrDefaultAsync(pl => pl.PackingListId == packingList.PackingListId);

                if (existingPackingList == null) throw new InvalidOperationException($"Packing List with ID {packingList.PackingListId} not found for update.");

                _context.Entry(existingPackingList).CurrentValues.SetValues(packingList);

                _context.PackingListItem.RemoveRange(existingPackingList.PackingListItems);
                await _context.SaveChangesAsync();

                foreach (var item in packingList.PackingListItems)
                {
                    item.PackingListId = existingPackingList.PackingListId;
                }
                await _context.PackingListItem.AddRangeAsync(packingList.PackingListItems);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}