using System.ComponentModel.DataAnnotations;

namespace cashTrackApi.Resources
{
  public class TransactionResource: TransactionDto
  {
    [Required]
    public string Id { get; set; }
  }
}
