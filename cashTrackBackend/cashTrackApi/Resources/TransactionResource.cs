using System;
using System.ComponentModel.DataAnnotations;

namespace cashTrackApi.Resources
{
  public class TransactionResource: TransactionDto
  {
    public string Id { get; set; }
  }
}
