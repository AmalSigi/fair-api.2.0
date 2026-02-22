using System.ComponentModel.DataAnnotations;

namespace FairMount_api.Models.Tables
{
  public class POitemStatus
  {
    [Key]
    public int ItemStatusId { get; set; }
    public string StatusName { get; set; }
    public string Description { get; set; }
    public string? ItemType { get; set; }
  }
}
