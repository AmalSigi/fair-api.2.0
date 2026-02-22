using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace FairMount_api.Models.Tables
{
    public class Sli_Item
    {
        [Key]
        public int Id { get; set; }

        public int SliId { get; set; }

        [Column("ItemId")]
        public int ItemId { get; set; }

        [Column("D/F")]
        public string DF { get; set; }

        [Column("ECCN/EAR99/USML")]
        public string EccnEar99Usml { get; set; }

        public string SME { get; set; }

        [Column("ELNo/NLR")]
        public string ElNoNlr { get; set; }

        public string PartNumber { get; set; }

        public string Description { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalPrice { get; set; }

        public int POId { get; set; }

        public string? HSC { get; set; }

        public string Unit { get; set; }

        public string LicenseValueByItem { get; set; }

        public decimal ShippingWeight { get; set; }

        public int LineNumber { get; set; }

        // 🔗 Navigation Property Back to Header 
        [JsonIgnore]
        [ForeignKey("SliId")]
        public virtual Sli_Document? Sli_Document { get; set; }
    }
}