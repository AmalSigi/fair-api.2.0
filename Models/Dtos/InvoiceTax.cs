namespace FairMount_api.Models.Dtos
{
    public class InvoiceTax
    {
        public int CommercialInvoiceId { get; set; }
        public string? CommercialInvoiceNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal ShippingCharge { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal NetCost{ get; set; }
    }
}





