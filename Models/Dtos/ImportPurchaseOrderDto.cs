using FairMount_api.Models.Tables;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FairMount_api.Models.Dtos
{
  public class ImportPurchaseOrderDto
  {
    [Required]
    public string PoNumber { get; set; }
    public string CustomerName { get; set; }

    public string? Supplier { get; set; }
    public string? Destination { get; set; }
    public string? PaymentTerms { get; set; }
    public string? DeliveryTerms { get; set; }

    public decimal? ShippingCharges { get; set; }
    public decimal? Discount { get; set; }

    [Required]
    public DateTime OrderDate { get; set; }

    public string? ModeOfShipment { get; set; }
    public string? DeliverySchedule { get; set; }

    public decimal TotalAmount { get; set; }
    public decimal TotalCost { get; set; }

    public string? Description { get; set; }

    public int? CreatedBy { get; set; }

    [Required]
    public List<ImportPOItemDto> Items { get; set; } = new();
  }

  public class ImportPOItemDto
  {
    public int Quantity { get; set; }
    public string? Unit { get; set; }
    public string? Description { get; set; }
    public string? ManufacturerModel { get; set; }
    public string? PartNumber { get; set; }
    public int TraceabilityRequired { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal ActualCostPerUnit { get; set; }
    public string? HSC{ get; set; }
    public string? CountryOfOrigin { get; set; }
    public string? WeightDim { get; set; }
    public int LineNumber { get; set; }
    public string? PoNumber { get; set; }
    }
}
