using System.ComponentModel.DataAnnotations;

namespace FairMount_api.Models.Tables
{
  public class PoStatus
  {
    [Key]
    public int StatusId { get; set; }
    public string? PoType { get; set; }
    public string? StatusName { get; set; }
    public string? Description { get; set; }
  }
}
