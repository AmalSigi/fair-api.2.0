namespace FairMount_api.Models.Dtos

{
    public class ElgiblePO
    {
        public int Id { get; set; }
        public string? PoNumber { get; set; }
        public int BuyerOrgId { get; set; }
        public DateTime OrderDate { get; set; }
        public int? StatusId { get; set; }
    }
}





