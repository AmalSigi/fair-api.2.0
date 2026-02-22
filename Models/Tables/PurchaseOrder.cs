using FairMount_api.Models.Tables;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class PurchaseOrders
{
    [Key]
    
    public int Id { get; set; }

    [Required]
    public string PoNumber { get; set; }

    [Required]
    public int CustomerId { get; set; }
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


    public int? CreatedBy { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
  public int? BuyerOrgId { get; set; }
  public int? VendorOrgId { get; set; }
  public int? Active { get; set; }
  public int? StatusId { get; set; }
  public int? PoTypeId { get; set; }
  // Navigation Properties
  [ForeignKey("CustomerId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [ValidateNever]
    public Customer Customer { get; set; }
    [ForeignKey("CreatedBy")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [ValidateNever]
    public User? User { get; set; }
  [ForeignKey("BuyerOrgId")]
  public Organizations? BuyerOrg { get; set; }
  [ForeignKey("VendorOrgId")]
  public Organizations? VendorOrg { get; set; }
  [ForeignKey("StatusId")]
  public PoStatus? PoStatus { get; set; }
  [ForeignKey("PoTypeId")]
  public Potype? PoType { get; set; }

}
