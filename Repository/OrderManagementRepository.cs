using FairMount_api.Data;
using FairMount_api.Interfaces;
using FairMount_api.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace FairMount_api.Repository
{
    public class OrderManagementRepository : IOrdermanagementRepository
    {
        private readonly FairmountDbContext _context;
        public OrderManagementRepository(FairmountDbContext context)
        {
            _context = context;
        }
        public async Task<PurchaseOrders> CreatePO(PurchaseOrders order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order), "Purchase order cannot be null");
            }

            // ADD 'await' HERE
            var existingPO = await _context.PurchaseOrders
                .FirstOrDefaultAsync(p => p.PoNumber.ToLower() == order.PoNumber.ToLower());

            if (existingPO != null)
            {
                throw new ArgumentException("Purchase order with the same PO number already exists");
            }

            // Logical assignments
            if (order.PoTypeId == 1)
            {
                order.StatusId = 1;
            }
            else
            {
                order.StatusId = 6;
            }

            _context.PurchaseOrders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }
        public async Task<List<PoDto>> GetActivePOS()
        {
            return await _context.PurchaseOrders
                .AsNoTracking()
                .Where(po => po.Active == 1)
                .OrderByDescending(po => po.CreatedAt)
                .Select(po => new PoDto
                {
                    Id = po.Id,
                    PoNumber = po.PoNumber,
                    Supplier = po.Supplier,
                    Destination = po.Destination,
                    PaymentTerms = po.PaymentTerms,
                    DeliveryTerms = po.DeliveryTerms,
                    ShippingCharges = po.ShippingCharges,
                    Discount = po.Discount,
                    OrderDate = po.OrderDate,
                    ModeOfShipment = po.ModeOfShipment,
                    DeliverySchedule = po.DeliverySchedule,
                    TotalAmount = po.TotalAmount,
                    TotalCost = po.TotalCost,
                    CreatedBy = po.CreatedBy,
                    CreatedAt = po.CreatedAt,

                    CustomerOrgName = po.Customer.CustomerName,
                    BuyerOrgName = po.BuyerOrg.Name,
                    VendorOrgName = po.VendorOrg.Name,
                    PoStatus = po.PoStatus.StatusName,
                    PoType = po.PoType.TypeName,
                    PoStatusId = po.StatusId
                })
                .ToListAsync();
        }

        //po items
        public async Task<List<POItem>> GetPOItems(int poId)
        {
            return await _context.POItems
                .AsNoTracking()
                .Where(item => item.PoId == poId)
                .ToListAsync();
        }
        public Task<int> AddPOItem(POItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "PO Item cannot be null");
            }
            _context.POItems.Add(item);
            return _context.SaveChangesAsync();
        }
        public async Task<int> UpdatePOItem(POItem item)
        {
            try
            {
                _context.POItems.Update(item);
                await _context.SaveChangesAsync();
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        //add items by creating new po
        public async Task<int> BulkAddPOItems(List<POItem> items, int POId, int? typeId, string? typeName)
        {
            List<POItem> exisistingItmes = await _context.POItems.Where(pi => items.Select(i => i.ItemId).Contains(pi.ItemId)).ToListAsync();
            if (typeId == null)
            {
                typeId = _context.POType.FirstOrDefault(t => t.TypeName.ToLower() == typeName.ToString().ToLower())?.TypeId;
            }

            var newPOItems = new List<POItem>();
            var affectedPos = new HashSet<int>();
            try
            {

                foreach (var item in items)
                {
                    var exisistingItem = exisistingItmes.FirstOrDefault(i => i.ItemId == item.ItemId);
                    if (exisistingItem != null)
                    {

                        if (exisistingItem.Quantity > 0)
                        {
                            affectedPos.Add(exisistingItem.PoId);
                            exisistingItem.Quantity -= item.Quantity;
                            exisistingItem.StatusId = 2;
                            _context.POItems.Update(exisistingItem);
                        }
                        if (exisistingItem.Quantity == 0) { exisistingItem.StatusId = 3; }

                        _context.POItems.Update(exisistingItem);

                        var newItem = new POItem
                        {
                            Quantity = item.Quantity,
                            Unit = item.Unit,
                            UnitPrice = item.UnitPrice,
                            Description = item.Description,
                            ManufacturerModel = item.ManufacturerModel,
                            ActualCostPerUnit = item.ActualCostPerUnit,
                            PartNumber = item.PartNumber,
                            TraceabilityRequired = item.TraceabilityRequired,
                            PoId = POId,
                            StatusId = typeId == 1 ? 5 : 1,
                            Terms = item.Terms,
                            HSC = item.HSC,
                            CountryOfOrigin = item.CountryOfOrigin,
                            WeightDim = item.WeightDim,
                        };
                        newPOItems.Add(newItem);

                    }
                    else
                    {
                        var newItem = new POItem
                        {
                            Quantity = item.Quantity,
                            Unit = item.Unit,
                            UnitPrice = item.UnitPrice,
                            Description = item.Description,
                            ManufacturerModel = item.ManufacturerModel,
                            ActualCostPerUnit = item.ActualCostPerUnit,
                            PartNumber = item.PartNumber,
                            PoId = POId,
                            TraceabilityRequired = item.TraceabilityRequired,
                            StatusId = typeId == 1 ? 5 : 1,
                            Terms = item.Terms,
                            HSC = item.HSC,
                            CountryOfOrigin = item.CountryOfOrigin,
                            WeightDim = item.WeightDim,
                        };
                        _context.POItems.Add(newItem);

                    }

                }
                await _context.POItems.AddRangeAsync(newPOItems);
                await _context.SaveChangesAsync();
                foreach (var id in affectedPos)
                {
                    await UpdatePOStatus(id);
                }
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }



        }
        //import purchase orders and items from csv
        public async Task<List<ImportPOResponseDto>> BulkImportPOS(List<ImportPurchaseOrderDto> pos)
        {
            var failedList = new List<ImportPOResponseDto>();
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var po in pos)
                {
                    if (string.IsNullOrWhiteSpace(po.PoNumber))
                    {
                        failedList.Add(new ImportPOResponseDto
                        {
                            Reason = "PO Number is missing",
                            PurchaseOrder = po,
                            Status = "Failed"

                        });
                        continue;
                    }
                    //check for exisiting po
                    var isPOExisisting = await _context.PurchaseOrders.FirstOrDefaultAsync(p => p.PoNumber.ToLower() == po.PoNumber.ToLower());

                    if (isPOExisisting != null)
                    {
                        failedList.Add(new ImportPOResponseDto
                        {
                            Reason = "PO already exists",
                            PurchaseOrder = po,
                            Status = "Failed"
                        });
                        continue;
                    }
                    //check for valid customer
                    var customer = await _context.Organizations.FirstOrDefaultAsync(c => c.Name.ToLower() == po.CustomerName.ToLower());
                    if (customer == null)
                    {
                        failedList.Add(new ImportPOResponseDto
                        {
                            Reason = "Customer does not exist",
                            PurchaseOrder = po,
                            Status = "Failed"
                        });
                        continue;

                    }
                    //create PO
                    var newPO = new PurchaseOrders
                    {
                        PoNumber = po.PoNumber,
                        CustomerId = customer != null ? customer.OrganizationId : 1,
                        BuyerOrgId = customer != null ? customer.OrganizationId : 1,
                        Supplier = po.Supplier,
                        Destination = po.Destination,
                        PaymentTerms = po.PaymentTerms,
                        DeliveryTerms = po.DeliveryTerms,
                        ShippingCharges = po.ShippingCharges,
                        Discount = po.Discount,
                        OrderDate = po.OrderDate,
                        ModeOfShipment = po.ModeOfShipment,
                        DeliverySchedule = po.DeliverySchedule,
                        TotalAmount = po.TotalAmount,
                        TotalCost = po.TotalCost,
                        StatusId = 1,
                        PoTypeId = 1,
                        CreatedBy = 1,
                        Active = 1
                    };
                    _context.PurchaseOrders.Add(newPO);
                    await _context.SaveChangesAsync();
                    //create PO items
                    var newItem = po.Items.Select(item => new POItem
                    {
                        Quantity = item.Quantity,
                        Unit = item.Unit,
                        Description = item.Description,
                        ManufacturerModel = item.ManufacturerModel,
                        PartNumber = item.PartNumber,
                        TraceabilityRequired = item.TraceabilityRequired,
                        UnitPrice = item.UnitPrice,
                        TotalPrice = item.TotalPrice,
                        ActualCostPerUnit = item.ActualCostPerUnit,
                        PoId = newPO.Id,
                        HSC = item.HSC,
                        CountryOfOrigin = item.CountryOfOrigin,
                        WeightDim = item.WeightDim,
                        LineNumber = item.LineNumber,
                        PoNumber = item.PoNumber,
                    }).ToList();
                    _context.POItems.AddRange(newItem);

                    failedList.Add(new ImportPOResponseDto
                    {
                        PurchaseOrder = po,
                        Status = "Success"
                    });

                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return failedList;

            }

            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return null;
            }
        }

        //customers
        public async Task<List<Customer>> GetCustomers()
        {
            return await _context.Customers
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<int> UpdatePOStatus(int poId)
        {
            try
            {
                var po = await _context.PurchaseOrders.FirstOrDefaultAsync(p => p.Id == poId);
                var poItems = await _context.POItems.Where(pi => pi.PoId == poId).ToListAsync();
                if (poItems.All(i => i.StatusId == 4 || i.StatusId == 8))
                {
                    if (po != null)
                    {
                        po.StatusId = po.PoTypeId == 1 ? 4 : 11;
                        _context.PurchaseOrders.Update(po);
                        await _context.SaveChangesAsync();
                    }
                }
                else if (poItems.Any(i => i.StatusId == 2 || i.StatusId == 6))
                {
                    if (po != null)
                    {
                        po.StatusId = 3;
                        _context.PurchaseOrders.Update(po);
                        await _context.SaveChangesAsync();
                    }
                }
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<bool> PoStatusUpdate(int statusId, int poId)
        {
            try
            {
                var po = await _context.PurchaseOrders.FirstOrDefaultAsync(p => p.Id == poId);

                if (po != null)
                {
                    po.StatusId = statusId;
                    _context.PurchaseOrders.Update(po);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }

}
