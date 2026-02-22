using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FairMount_api.Models.Tables
{
    [Table("PackingListItem")]
    public class PackingListItem
    {
        [Key]
        public int Id { get; set; }   // Auto ID (PK)

        [Column("PackingListId")]
        [Required]
        public int PackingListId { get; set; }

        [Column("ItemId")]
        public int ItemId { get; set; }

        [Column("PartNumber")]
        public string? PartNumber { get; set; }

        [Column("Description")]
        public string? Description { get; set; }

        [Column("PONumber")]
        public string? PONumber { get; set; }

        [Column("Quantity")]
        [Required]
        public int Quantity { get; set; }

        [Column("UnitPrice")]
        public decimal? UnitPrice { get; set; }

        [Column("TotalPrice")]
        public decimal? TotalPrice { get; set; }

        [Column("CountryOfOrigin")]
        public string? CountryOfOrigin { get; set; }

        [Column("Unit")]
        public string? Unit { get; set; }

        [Column("POId")]
        public int? POId { get; set; }

        [Column("HSC")]
        public string? HSC { get; set; }

        [Column("WeightDim")]
        public string? WeightDim { get; set; }

        [Column("LineNumber")]
        public int LineNumber { get; set; }

        // 🔗 Navigation Property
        [JsonIgnore]
        [ForeignKey(nameof(PackingListId))]
        public virtual PackingList? PackingList { get; set; }
    }
}
