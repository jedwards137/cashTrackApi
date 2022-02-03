using System;
using System.ComponentModel.DataAnnotations;

namespace cashTrackApi.Resources
{
  public class TransactionDto
  {
    [Required]
    public string UserId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    [StringLength(50)]
    public string Category { get; set; }

    [Required]
    [StringLength(50)]
    public string SubCategory { get; set; }

    [Required]
    [StringLength(50)]
    public string Account { get; set; }

    [Required]
    public double Amount { get; set; }

    [Required]
    [StringLength(10)]
    public string Type { get; set; }
  }
}
