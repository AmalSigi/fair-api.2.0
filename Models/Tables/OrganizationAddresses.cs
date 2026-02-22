using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FairMount_api.Models.Tables
{
  public class OrganizationAddresses
  {
    [Key]
    public int AddressId { get; set; }

    [ForeignKey(nameof(Organization))]
    public int OrganizationId { get; set; }

    [StringLength(50)]
    public string? AddressType { get; set; } // "Billing", "Shipping", "HeadOffice"

    [StringLength(255)]
    public string? AddressLine1 { get; set; }

    [StringLength(255)]
    public string? AddressLine2 { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(100)]
    public string? State { get; set; }

    [StringLength(20)]
    public string? PostalCode { get; set; }

    [StringLength(100)]
    public string? Country { get; set; }

    // Navigation Propertyds
    [JsonIgnore]
    public Organizations? Organization { get; set; }
  
}
}
