using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FairMount_api.Models.Tables
{
    [Table("CommercialInvoiceItem")]
    public class CommercialInvoiceItem
    {
        [Key]
        public int Id { get; set; }

       

        public int CommercialInvoiceId { get; set; }

        [Column("ItemId")]
        [Required]
        public int ItemId { get; set; }

        [Column("PartNumber")]
        [Required]
        public string PartNumber { get; set; } = string.Empty;
        [Column("CommercialInvoiceNumber")]
        [Required]
        public string CommercialInvoiceNumber { get; set; } = string.Empty;

        

        [Column("Description")]
        public string? Description { get; set; }

      
        [Column("PONumber")]
        public string? PONumber { get; set; }

        [Column("CountryOfOrigin")]
        public string? CountryOfOrigin { get; set; }

        [Column("Unit")]
        public string? Unit { get; set; }

        [Column("Quantity")]
        [Required]
        public int Quantity { get; set; }

        [Column("POId")]
        [Required]
        public int PoId { get; set; }


        [Column("UnitPrice")]
        [Required]
        public decimal UnitPrice { get; set; }

        [Column("TotalPrice")]
        [Required]
        public decimal TotalPrice { get; set; }

        [Column("LineNumber")]
        public int LineNumber { get; set; }


        // 🔗 Navigation Property Back to Header 
        [JsonIgnore]
        public virtual CommercialInvoice? CommercialInvoice { get; set; }
    }
}