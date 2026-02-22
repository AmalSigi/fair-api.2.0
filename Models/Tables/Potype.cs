using System.ComponentModel.DataAnnotations;

namespace FairMount_api.Models.Tables
{
  public class Potype
  {
    [Key]
   public int TypeId { get; set; }
    public string TypeName { get; set; }

  }
}
