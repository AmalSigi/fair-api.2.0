namespace FairMount_api.Models.Dtos
{
  public class ImportPOResponseDto
  {
    public string? Reason { get; set; }= string.Empty;
    public string? Status { get; set; } = string.Empty;
    public ImportPurchaseOrderDto? PurchaseOrder { get; set; }
  }
}
