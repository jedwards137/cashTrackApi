using cashTrackApi.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace cashTrackApi
{
  public static class GetTransactionsAll
  {
    [FunctionName("GetTransactionsAll")]
    public static async Task<IActionResult> Run(
      [HttpTrigger(AuthorizationLevel.Function, "get", Route = "transactions")] HttpRequest req, ILogger log)
    {
      var transactionList = new List<TransactionResource>();
      var dbConnectionStr = Environment.GetEnvironmentVariable("sqldb_connection");
      var query = "exec [dbo].[getTransactionsAll]";

      try
      {
        using SqlConnection conn = new SqlConnection(dbConnectionStr);
        conn.Open();
        using SqlCommand cmd = new SqlCommand(query, conn);
        var reader = await cmd.ExecuteReaderAsync();
        while (reader.Read())
        {
          var transaction = new TransactionResource()
          {
            TransactionId = (int)reader["TransactionId"],
            Date = (DateTime)reader["Date"],
            Description = reader["Description"].ToString(),
            Category = reader["Category"].ToString(),
            Account = reader["Account"].ToString(),
            Amount = (double)reader["Amount"]
          };
          transactionList.Add(transaction);
        }
      }
      catch (Exception e)
      {
        log.LogError($"Error executing GetAllTransactions :: {e}");
        return new BadRequestResult();
      }

      return new OkObjectResult(transactionList);
    }
  }
}
