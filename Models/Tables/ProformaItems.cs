using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FairMount_api.Models.Tables
{
    public class ProformaItems
    {
        [Key]
        public int ItemId { get; set; }

        [Required]
        public int ProformaId { get; set; }

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
       
        public int LineNumber { get; set; }

        // 🔗 Navigation Property back to parent
        [JsonIgnore]
        [ForeignKey("ProformaId")]
        public virtual ProformaInvoice? ProformaInvoice { get; set; }
    }
}