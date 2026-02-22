using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FairMount_api.Models.Tables
{
    public class ProformaInvoice
    {
        [Key]
        public int ProformaId { get; set; }

        [Required]
        public string ProformaNumber { get; set; }

        public int PurchaseOrderId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        public int? ShipToId { get; set; }

        [MaxLength(100)]
        public string? FreightType { get; set; }

        [MaxLength(10)]
        public string? CurrencyCode { get; set; } = "USD";

        public decimal TotalAmount { get; set; }

        public string? Notes { get; set; }

        public DateTime? EstimatedShipDate { get; set; }

        public string? Status { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? CustomerAddressId { get; set; }

        public string? PoNumber { get; set; }


        // 🔗 Navigation Properties - Updated with Explicit Foreign Keys
        [JsonIgnore]
        [ForeignKey("PurchaseOrderId")] // Explicitly links to PurchaseOrderId column
        public virtual PurchaseOrders? PurchaseOrder { get; set; }

        [JsonIgnore]
        [ForeignKey("CustomerId")] // Explicitly links to CustomerId column
        public virtual Organizations? Customer { get; set; }

        [JsonIgnore]
        [ForeignKey("ShipToId")] // FIX: Links the Address property to your ShipToId column
        public virtual OrganizationAddresses? AddressId { get; set; }

        public virtual ICollection<ProformaItems> ProformaItems { get; set; }
    }
}