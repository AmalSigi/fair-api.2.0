using FairMount_api.Models.Tables;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class POItem
{
  [Key]

  public int ItemId { get; set; }

  [Required]
  public int PoId { get; set; }


  public int Quantity { get; set; }

  public string? Unit { get; set; }

  public string? Description { get; set; }


  public string? ManufacturerModel { get; set; }

  public string? PartNumber { get; set; }

  public int TraceabilityRequired { get; set; } = 1;

  public decimal UnitPrice { get; set; }

  public decimal TotalPrice { get; set; }

  public decimal ActualCostPerUnit { get; set; }
  public int? StatusId { get; set; }
  public string? Terms { get; set; }
  public string? CountryOfOrigin { get; set; }
  public string? HSC { get; set; }
  public string? WeightDim { get; set; }
  public int LineNumber { get; set; }
  public string? PoNumber { get; set; }
  public int? InvoicedQty { get; set; }


    [ForeignKey("StatusId")]
  public virtual POitemStatus? ItemStatus { get; set; }


}

