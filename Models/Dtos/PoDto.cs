using FairMount_api.Models.Tables;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FairMount_api.Models.Dtos
{
  public class PoDto
  {
    public int Id { get; set; }
    public string PoNumber { get; set; }
    public int CustomerId { get; set; }
    public string? Supplier { get; set; }
    public string? Country { get; set; }
    public string? PaymentTerms { get; set; }
    public string? DeliveryTerms { get; set; }
    public decimal? ShippingCharges { get; set; }
    public decimal? Discount { get; set; }
    public DateTime OrderDate { get; set; }

    public string? ModeOfShipment { get; set; }

    public string? DeliverySchedule { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal TotalCost { get; set; }


    public string Description { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
        
    public string? CustomerOrgName { get; set; }
    public string? BuyerOrgName { get; set; }
    public string? VendorOrgName { get; set; }
    public int? Active { get; set; }
    public string? PoStatus { get; set; }
    public string? PoType { get; set; }
    public int?PoStatusId { get; set; }
  }
}
