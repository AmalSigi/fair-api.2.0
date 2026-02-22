using FairMount_api.Application.Interfaces;
using FairMount_api.Data;
using FairMount_api.Models.Tables;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FairMount_api.Repository
{
    public class CommercialInvoiceRepository : ICommercialInvoiceRepository
    {
        private readonly FairmountDbContext _context;

        public CommercialInvoiceRepository(FairmountDbContext context)
        {
            _context = context;
        }

        public async Task<bool> InvoiceExistsAsync(string invoiceNumber)
        {
            return await _context.CommercialInvoices.AnyAsync(i => i.CommercialInvoiceNumber == invoiceNumber);
        }

        // Get single invoice by CommercialInvoiceNumber
        public async Task<CommercialInvoice?> GetInvoiceAsync(int invoiceId)
        {
            return await _context.CommercialInvoices
                .AsNoTracking()
                .Include(ci => ci.CommercialInvoiceItems)
                .Include(ci => ci.PackingList)
                    .ThenInclude(pl => pl.PackingListItems)
                .Include(ci => ci.Sli_Document)
                .FirstOrDefaultAsync(ci => ci.CommercialInvoiceId == invoiceId);
        }

        public async Task<List<CommercialInvoice>> GetAllInvoicesAsync(DateOnly? startDate, DateOnly? endDate)
        {
            IQueryable<CommercialInvoice> query = _context.CommercialInvoices.AsNoTracking();

            if (startDate.HasValue)
            {
                var startDateTime = startDate.Value.ToDateTime(TimeOnly.MinValue);
                query = query.Where(pi => pi.CreatedAt >= startDateTime);
            }

            if (endDate.HasValue)
            {
                var endDateTime = endDate.Value.ToDateTime(TimeOnly.MaxValue);
                query = query.Where(pi => pi.CreatedAt <= endDateTime);
            }

            // Use ToListAsync() to ensure the call is actually non-blocking
            return await query.OrderByDescending(ci => ci.CreatedAt).ToListAsync();
        }
        // --- Create Invoice  ---



        public async Task<CommercialInvoice> CreateInvoiceAsync(CommercialInvoice invoice)
        {
            const int PARTIALLY_INVOICED = 12;
            const int FULLY_INVOICED = 10;

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Save invoice
                _context.CommercialInvoices.Add(invoice);
                await _context.SaveChangesAsync();

                // 2. Update invoiced qty for PO items
                foreach (var invItem in invoice.CommercialInvoiceItems)
                {
                    if (invItem.PoId <= 0)
                        continue;

                    var poItem = await _context.POItems
             .Where(p => p.PoId == invItem.PoId &&
                         p.ItemId == invItem.ItemId)
             .AsTracking()
             .FirstOrDefaultAsync();

                    if (poItem == null)
                        continue;
                    poItem.InvoicedQty = (poItem.InvoicedQty ?? 0) + invItem.Quantity;
                }

                var rows = await _context.SaveChangesAsync();
                Console.WriteLine($"Rows affected: {rows}");

                // 3. Get affected PO IDs
                var poIds = invoice.CommercialInvoiceItems
                    .Where(x => x.PoId > 0)
                    .Select(x => x.PoId)
                    .Distinct()
                    .ToList();

                // 4. Recalculate PO status
                foreach (var poId in poIds)
                {
                    var poItems = await _context.POItems
                        .Where(p => p.PoId == poId)
                        .ToListAsync();

                    bool allFullyInvoiced = poItems
                        .All(i => i.InvoicedQty >= i.Quantity);

                    var newStatus = allFullyInvoiced
                        ? FULLY_INVOICED
                        : PARTIALLY_INVOICED;

                    await _context.PurchaseOrders
                        .Where(po => po.Id == poId)
                        .ExecuteUpdateAsync(setters => setters
                            .SetProperty(po => po.StatusId, newStatus));
                }

                await transaction.CommitAsync();
                return invoice;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateInvoiceAsync(CommercialInvoice invoice)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingInvoice = await _context.CommercialInvoices
                    .Include(ci => ci.CommercialInvoiceItems)
                    .FirstOrDefaultAsync(ci => ci.CommercialInvoiceNumber == invoice.CommercialInvoiceNumber);

                if (existingInvoice == null) throw new InvalidOperationException("Invoice not found for update.");

                invoice.CommercialInvoiceId = existingInvoice.CommercialInvoiceId;

                _context.Entry(existingInvoice).CurrentValues.SetValues(invoice);

                _context.CommercialInvoiceItems.RemoveRange(existingInvoice.CommercialInvoiceItems);
                await _context.SaveChangesAsync();

                foreach (var item in invoice.CommercialInvoiceItems)
                {
                    item.CommercialInvoiceId = existingInvoice.CommercialInvoiceId;
                }
                await _context.CommercialInvoiceItems.AddRangeAsync(invoice.CommercialInvoiceItems);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // --- Get Eligible POs ---
        public async Task<IEnumerable<object>> GetEligiblePurchaseOrdersAsync()
        {
            const int REQUIRED_PO_TYPE = 1;
            const int REQUIRED_STATUS_ID = 4;

            return await _context.PurchaseOrders
                .AsNoTracking() // Crucial for read-only "Get" calls
                .Where(po => po.PoTypeId == REQUIRED_PO_TYPE && (po.StatusId == REQUIRED_STATUS_ID || po.StatusId == 12))
                .Select(po => new
                {
                    po.Id, // Shorthand assignment
                    po.PoNumber,
                    po.BuyerOrgId,
                    po.OrderDate,
                    CurrentStatus = po.StatusId.ToString()
                })
                .ToListAsync(); // Changed from ToListAsync casting to direct call
        }

        public async Task<bool> DeleteComericalInvoiceAsync(int id, int poId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try { 
                var invoice = await _context.CommercialInvoices
                    .Include(i => i.CommercialInvoiceItems)
                    .Include(i => i.PackingList)
                        .ThenInclude(p => p.PackingListItems)
                    .Include(i => i.Sli_Document)    // Include the SLI Items
                    .FirstOrDefaultAsync(i => i.CommercialInvoiceId == id);

                if (invoice == null) return false;

                var po = await _context.PurchaseOrders.FirstOrDefaultAsync(o => o.Id == poId);
                if (po != null)
                {
                    po.StatusId = 4;
                }
                else
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                _context.CommercialInvoices.Remove(invoice);
                _context.PurchaseOrders.Update(po);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
    }
}