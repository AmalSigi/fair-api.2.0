using System.ComponentModel.DataAnnotations;

namespace FairMount_api.Models.Dtos
{
    // DTOs/SLI/CreateSLIDto.cs
    public class CreateSLIDto
    {
        // USPPI Information
        public string SliNumber { get; set; }
        public int UsppiNameId { get; set; }
        public int UsppiAddressId { get; set; }
        public string FLCName { get; set; }
        public string FLCAddress { get; set; }
        public string ForwardingAgent { get; set; }
        public string UsppiEin { get; set; }
        public bool RelatedPartyIndicator { get; set; }
        public string UsppiReference { get; set; }
        public bool RoutedExportTransaction { get; set; }

        // Consignee Information
        public int UCName { get; set; }
        public int UCAddressId { get; set; }
        public string UCType { get; set; }
        public string ICName { get; set; }
        public string ICAddress { get; set; }
        public string StateOfOrigin { get; set; }
        public string CountryOfUltimateDestination { get; set; }

        // Regulatory Information
        public bool HazardousMaterial { get; set; }
        public string InBondCode { get; set; }
        public string EntryNumber { get; set; }
        public bool TIBCarnet { get; set; }
        public string DDTCApplicantRegistrationNumber { get; set; }
        public bool EligiblePartyCertification { get; set; }
        public bool NonLicensableScheduleB { get; set; }
        public bool UsppiAuthorize { get; set; }
        public string UsppiEmailAddress { get; set; }
        public string AuthorizedOfficerName { get; set; }
        public string OfficerTitle { get; set; }
        public bool ValidateElectronicSignature { get; set; }

        // Related Documents
        public int? CommercialInvoiceId { get; set; }
        public int? PackingListId { get; set; }

        // Items
        public List<CreateSLIItemDto> Items { get; set; } = new();
    }

    // DTOs/SLI/CreateSLIItemDto.cs
    public class CreateSLIItemDto
    {
        public int ItemId { get; set; }
        public string DF { get; set; }
        public string EccnEar99Usml { get; set; }
        public string SME { get; set; }
        public string ElNoNlr { get; set; }
        public string PartNumber { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; } // Match DB typo
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public int POId { get; set; }
        public string? HSC { get; set; }
        public string UI { get; set; }
        public string LicenseValueByItem { get; set; }
        public decimal ShippingWeight { get; set; }
    }

    // DTOs/SLI/SLIResponseDto.cs
    public class SLIResponseDto
    {
        public int SliId { get; set; }
        public string SliNumber { get; set; }
        public string FLCName { get; set; }
        public string UsppiEmailAddress { get; set; }
        public List<SLIItemResponseDto> Items { get; set; } = new();
        public decimal TotalInvoiceValue => Items.Sum(i => i.TotalPrice);
        public DateTime CreatedAt { get; set; }
    }

    // DTOs/SLI/SLIItemResponseDto.cs
    public class SLIItemResponseDto
    {
        public string PartNumber { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}