using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FairMount_api.Models.Tables
{
    [Table("CommercialInvoice")]
    public class CommercialInvoice
    {
        [Key]
        public int CommercialInvoiceId { get; set; }

        [Required]
        [Column("CommercialInvoiceNumber")]
        public string CommercialInvoiceNumber { get; set; } = string.Empty;


        [Column("CustomerOrgId")]
        [Required]
        public int CustomerId { get; set; }
        [Column("CreatedBy")]
        [Required]
        public int CreatedBy { get; set; }
        

        [Column("ShipperOrgId")]
        [Required]
        public int ShipperId { get; set; }

        [Column("ShipToAddress")] // Maps to SQL 'ShipToAddress'
        [Required]
        public int ShipToId { get; set; } // C# uses Id suffix for clarity
        [Column("customerAddressesId")]
        public int? CustomerAddressesId { get; set; }
        // General Invoice Details (Properties renamed for clarity, mapped to new SQL names)
        [Column("CreatedAt")] // Maps to SQL 'CreatedAt'
        [Required]
        public DateTime CreatedAt { get; set; }

        [Column("Currency")]
        public string? Currency { get; set; } = "USD";

        [Column("TotalAmount")]
        public decimal TotalAmount { get; set; }

       
        [Column("TermsOfSale")]
        public string? TermsOfSale { get; set; }

        [Column("TermsOfPayment")]
        public string? TermsOfPayment { get; set; }

        [Column("TermsOfShipping")]
        public string? TermsOfShipping { get; set; }

        [Column("ModeOfTransport")]
        public string? ModeOfTransport { get; set; }

        [Column("PlaceOfReceipt")] // Maps to SQL 'PlaceOfRReceipt' (User's requested SQL name)
        public string? PlaceOfReceipt { get; set; } // C# uses correct spelling

        [Column("FinalDestination")]
        public string? FinalDestination { get; set; }

     
        [Column("ContactName")]
        public string? ContactName { get; set; }

        [Column("ContactNo")]
        public string? ContactNo { get; set; }

        [Column("TaxId")]
        public string? TaxId { get; set; }

        [Column("Ein")]
        public string? Ein { get; set; }


        [Column("Email")]
        public string? Email { get; set; }

        [Column("Note")]
        public string? Note{ get; set; }


        // 🔗 Navigation Properties 
        // NOTE: These entities ( Organizations, OrganizationAddresses) 
        // MUST have corresponding DbSets in FairmountDbContext.cs to compile.

        [JsonIgnore]
        [ForeignKey(nameof(CustomerId))]
        public virtual Organizations? CustomerOrg { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(ShipperId))]
        public virtual Organizations? ShipperOrg { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(ShipToId))]
        public virtual OrganizationAddresses? ShipToAddress { get; set; }

        public ICollection<CommercialInvoiceItem> CommercialInvoiceItems { get; set; } = new List<CommercialInvoiceItem>();
        public ICollection<PackingList> PackingList { get; set; } = new List<PackingList>();
        public ICollection<Sli_Document> Sli_Document { get; set; } = new List<Sli_Document>();



    }
}