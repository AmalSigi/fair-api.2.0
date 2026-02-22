using FairMount_api.Data;
using FairMount_api.Interfaces;
using FairMount_api.Models.Tables;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace FairMount_api.Repository
{
  public class InvoicesRepository : IInvoiceRepository
  {
    private readonly FairmountDbContext _context;
    public InvoicesRepository(FairmountDbContext context)
    {
      _context = context;
    }


        #region Proforma Invoices
        /// Get all profroma invoiceable pos
        public async Task<List<PurchaseOrders>> GetAllProformaInvoiceablePOsAsync()
        {
            return await _context.PurchaseOrders
                .AsNoTracking() 
                .Where(po => po.StatusId == 1 && po.PoTypeId == 1)
                .ToListAsync();
        }
        /// <summary>
        /// Get all proforma invoices within the specified date range.
        /// if no dates are provided, return all proforma invoices.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<List<ProformaInvoice>> GetAllProformaInvoicesAsync(DateOnly? startDate, DateOnly? endDate)
        {
            IQueryable<ProformaInvoice> query = _context.ProformaInvoices
                .AsNoTracking() // Prevents EF from tracking changes in memory
                .Include(pi => pi.PurchaseOrder)
                .Include(pi => pi.Customer)
                .Include(pi => pi.AddressId);

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

            return await query.ToListAsync();
        }

        public async Task<ProformaInvoice> CreateProformaInvoiceAsync(ProformaInvoice proformaInvoice)
        {
            proformaInvoice.Status = "Pending";

            _context.ProformaInvoices.Add(proformaInvoice);

            var purchaseOrder = await _context.PurchaseOrders
                .FirstOrDefaultAsync(po => po.Id == proformaInvoice.PurchaseOrderId);

            if (purchaseOrder != null)
            {
                purchaseOrder.StatusId = 2;
                _context.PurchaseOrders.Update(purchaseOrder);
            }

            await _context.SaveChangesAsync();

            return proformaInvoice;
        }

        public async Task<ProformaInvoice?> GetProformaInvoiceByIdAsync(int proformaId)
        {
            return await _context.ProformaInvoices
                .AsNoTracking()
                .Include(pi => pi.PurchaseOrder)
                .Include(pi => pi.Customer)
                .Include(pi => pi.AddressId)
                .Include(pi => pi.ProformaItems)
                .FirstOrDefaultAsync(pi => pi.ProformaId == proformaId);
        }
        public async Task<ProformaInvoice> UpdateProformaInvoiceStatusAsync(int id, int poId)
        {
            // Start a transaction to ensure data consistency
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var proPo = await _context.ProformaInvoices.FirstOrDefaultAsync(po => po.ProformaId == id);
                var po = await _context.PurchaseOrders.FirstOrDefaultAsync(po => po.Id == poId);

                if (proPo != null && po != null)
                {
                    proPo.Status = "Complete";
                    po.StatusId = 4;

                    _context.PurchaseOrders.Update(po);
                    _context.ProformaInvoices.Update(proPo);

                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return proPo;
                }

                return null;
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();

                throw new Exception("Database update failed. Changes rolled back.", ex);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        public async Task<bool> DeleteProformaInvoiceAsync(int id, int poId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var proforma = await _context.ProformaInvoices
                    .Include(o => o.ProformaItems)
                    .FirstOrDefaultAsync(o => o.ProformaId == id);

                if (proforma == null) return false;

                var po = await _context.PurchaseOrders.FirstOrDefaultAsync(o => o.Id == poId);
                if (po != null)
                {
                    po.StatusId = 1;
                }
                else
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                _context.ProformaInvoices.Remove(proforma);
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
#endregion
