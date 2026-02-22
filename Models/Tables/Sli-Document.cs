using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FairMount_api.Models.Tables
{
    public class Sli_Document
    {
        [Key]
        public int SliId { get; set; } 

        public int UsppiNameId { get; set; } 
        public int UsppiAddressId { get; set; } 

        [StringLength(255)]
        public string FLCName { get; set; } 

        [StringLength(255)]
        public string FLCAddress { get; set; } 

        [StringLength(255)]
        public string ForwardingAgent { get; set; } 

        [StringLength(255)]
        public string UsppiEin { get; set; }

        public bool RelatedPartyIndicator { get; set; } 

        [StringLength(255)]
        public string UsppiReference { get; set; } 

        public bool RoutedExportTransaction { get; set; } 

        public int UCName { get; set; }
        public int UCAddressId { get; set; } 

        [StringLength(255)]
        public string UCType { get; set; }

        [StringLength(255)]
        public string ICName { get; set; }

        [StringLength(255)]
        public string ICAddress { get; set; } 
        [StringLength(255)]
        public string StateOfOrigin { get; set; } 

        [StringLength(255)]
        public string CountryOfUltimateDestination { get; set; }

     
        public bool HazardousMaterial { get; set; }

        [StringLength(255)]
        public string InBondCode { get; set; } 

        [StringLength(255)]
        public string EntryNumber { get; set; } 

        public bool TIBCarnet { get; set; } 

        [StringLength(255)]
        public string DDTCApplicantRegistrationNumber { get; set; }

        public bool EligiblePartyCertification { get; set; }
        public bool NonLicensableScheduleBHTSNumbers{ get; set; }
        public bool UsppiAuthorize { get; set; } 

        [StringLength(255)]
        public string UsppiEmailAddress { get; set; } 

        [StringLength(255)]
        public string AuthorizedOfficerName { get; set; }

        [StringLength(255)]
        public string OfficerTitle { get; set; }

        public bool ValidateElectronicSignature { get; set; } 
        public int? CommercialInvoiceId { get; set; }
        public int? PackingListId { get; set; }
        public string CommercialInvoiceNumber{ get; set; }
        public string FtzIdentifier{ get; set; }



        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<Sli_Item> Items { get; set; } = new();
    }
}