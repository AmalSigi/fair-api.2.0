using System.ComponentModel.DataAnnotations;

namespace FairMount_api.Models.Tables
{
  public class Organizations
  {
    [Key]
    public int OrganizationId { get; set; }

    [Required, StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string Type { get; set; } = string.Empty; // "Customer", "Vendor", or "Both"

    [StringLength(255)]
    public string? Email { get; set; }

    [StringLength(50)]
    public string? Phone { get; set; }

    [StringLength(100)]
    public string? TaxId { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Property
    public ICollection<OrganizationAddresses>? Addresses { get; set; }
  
}
}
