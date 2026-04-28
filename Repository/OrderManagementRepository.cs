using FairMount_api.Data;
using FairMount_api.Interfaces;
using FairMount_api.Models.Dtos;
using FairMount_api.Models.Tables;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.SymbolStore;
using System.Reflection.Metadata.Ecma335;
using MySql.Data.MySqlClient;
using System.Data;
using Dapper;

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
        //public async Task<List<PoDto>> GetActivePOS()
        //{
        //    return await _context.PurchaseOrders
        //        .AsNoTracking()
        //        .Where(po => po.Active == 1)
        //        .OrderByDescending(po => po.CreatedAt)
        //        .Select(po => new PoDto
        //        {
        //            Id = po.Id,
        //            PoNumber = po.PoNumber,
        //            Supplier = po.Supplier,
        //            Destination = po.Destination,
        //            PaymentTerms = po.PaymentTerms,
        //            DeliveryTerms = po.DeliveryTerms,
        //            ShippingCharges = po.ShippingCharges,
        //            Discount = po.Discount,
        //            OrderDate = po.OrderDate,
        //            ModeOfShipment = po.ModeOfShipment,
        //            DeliverySchedule = po.DeliverySchedule,
        //            TotalAmount = po.TotalAmount,
        //            TotalCost = po.TotalCost,
        //            CreatedBy = po.CreatedBy,
        //            CreatedAt = po.CreatedAt,

        //            CustomerOrgName = po.Customer.CustomerName,
        //            BuyerOrgName = po.BuyerOrg.Name,
        //            VendorOrgName = po.VendorOrg.Name,
        //            PoStatus = po.PoStatus.StatusName,
        //            PoType = po.PoType.TypeName,
        //            PoStatusId = po.StatusId
        //        })
        //        .ToListAsync();
        //}


        public async Task<List<PoDto>> GetActivePOS()
        {
            var connection = _context.Database.GetDbConnection();
            var result = await connection.QueryAsync<PoDto>(
                "GetActivePurchaseOrders",
                commandType: CommandType.StoredProcedure
            );
            return result.ToList();
        }
        //po items
        public async Task<List<POItem>> GetPOItems(int poId)
        {
            using var connection = _context.Database.GetDbConnection();

            if (connection.State == ConnectionState.Closed)
                await connection.OpenAsync();

            var result = await connection.QueryAsync<POItem>(
                "GetPOItems",
                new { poId },
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
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

                        if (exisistingItem.RemainingQuantity > 0)
                        {
                            affectedPos.Add(exisistingItem.PoId);
                            exisistingItem.RemainingQuantity -= item.Quantity;
                            exisistingItem.TakenQuantity += item.Quantity;
                            exisistingItem.StatusId = 4;
                            _context.POItems.Update(exisistingItem);
                        }
                        if (exisistingItem.RemainingQuantity == 0) { exisistingItem.StatusId = 4; }

                        _context.POItems.Update(exisistingItem);

                        var newItem = new POItem
                        {
                            Quantity = item.Quantity,
                            LineNumber = item.LineNumber,
                            PoNumber = item.PoNumber,
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
                            Discount = item.Discount,
                            RemainingQuantity = item.Quantity,
                        };
                        newPOItems.Add(newItem);

                    }
                    else
                    {
                        var newItem = new POItem
                        {
                            Quantity = item.Quantity,
                            LineNumber = item.LineNumber,
                            PoNumber = item.PoNumber,
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
                            Discount = item.Discount,
                            RemainingQuantity = item.Quantity
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
                        OrderDate = po.OrderDate,
                        DeliverySchedule = po.DeliverySchedule,
                        Destination = po.Destination,
                        PaymentTerms = po.PaymentTerms,
                        ShippingCharges = po.ShippingCharges,
                        DeliveryTerms = po.DeliveryTerms,
                        ModeOfShipment = po.ModeOfShipment,
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
                        LineNumber = item.LineNumber,
                        Unit = item.Unit,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        ActualCostPerUnit = item.ActualCostPerUnit,
                        Discount = item.Discount,
                        ManufacturerModel = item.ManufacturerModel,
                        PartNumber = item.PartNumber,
                        Description = item.Description,
                        CountryOfOrigin = item.CountryOfOrigin,
                        TraceabilityRequired = item.TraceabilityRequired,
                        HSC = item.HSC,
                        WeightDim = item.WeightDim,
                        PoId = newPO.Id,
                        PoNumber = item.PoNumber,
                        RemainingQuantity = item.Quantity,
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

        public async Task<PurchaseOrders> PoUpdate(int poId, PurchaseOrders po)
        {
            try
            {
                var existingPO = await _context.PurchaseOrders.FirstOrDefaultAsync(p => p.Id == poId);
                if (existingPO == null)
                {
                    return null;
                }
                existingPO.PoNumber = po.PoNumber;
                existingPO.PoTypeId = po.PoTypeId ?? existingPO.PoTypeId;
                existingPO.Destination = po.Destination ?? existingPO.Destination;
                existingPO.PaymentTerms = po.PaymentTerms ?? existingPO.PaymentTerms;
                existingPO.DeliveryTerms = po.DeliveryTerms ?? existingPO.DeliveryTerms;
                existingPO.ShippingCharges = po.ShippingCharges ?? existingPO.ShippingCharges;
                if (po.OrderDate != default(DateTime))
                {
                    existingPO.OrderDate = po.OrderDate;
                }
                existingPO.ModeOfShipment = po.ModeOfShipment ?? existingPO.ModeOfShipment;
                existingPO.DeliverySchedule = po.DeliverySchedule ?? existingPO.DeliverySchedule;
                _context.PurchaseOrders.Update(existingPO);
                await _context.SaveChangesAsync();
                return existingPO;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<PODetails>> GetPODetails(int poId)
        {
            var connection = _context.Database.GetDbConnection();
            var result = await connection.QueryAsync<PODetails>(
                "GetPODetails",
                new { poId },
                commandType: CommandType.StoredProcedure
            );
            return result.ToList();
        }
    }

}
