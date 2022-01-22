using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Collections.Generic;
using cashTrackApi.Resources;

namespace cashTrackApi
{
  public static class GetTransactionsByMonth
  {
    [FunctionName("GetTransactionsByMonth")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "transactions/month/{monthId}")] HttpRequest req, string monthId, ILogger log)
    {
      var transactionsList = new List<TransactionResource>();

      try
      {
        var monthNumber = int.Parse(monthId);
        var dbConnectionStr = Environment.GetEnvironmentVariable("sqldb_connection");
        var query = $"exec [dbo].[getTransactionsByMonth] @monthId='{monthNumber}'";

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
          transactionsList.Add(transaction);
        }
      }
      catch (Exception e)
      {
        log.LogError($"Error executing GetMonthTransactions :: {e}");
        return new BadRequestResult();
      }

      return new OkObjectResult(transactionsList);
    }
  }
}
