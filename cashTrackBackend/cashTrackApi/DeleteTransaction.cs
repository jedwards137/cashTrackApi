using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace cashTrackApi
{
  public static class DeleteTransaction
  {
    [FunctionName("DeleteTransaction")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "transactions/{tId}")] HttpRequest req, string tId, ILogger log)
    {
      try
      {
        var transactionId = int.Parse(tId);
        var dbConnectionStr = Environment.GetEnvironmentVariable("sqldb_connection");
        var query = $"exec [dbo].[deleteTransaction] @transactionId={transactionId}";

        using SqlConnection conn = new SqlConnection(dbConnectionStr);
        conn.Open();
        using SqlCommand cmd = new SqlCommand(query, conn);
        var rows = await cmd.ExecuteNonQueryAsync();
        return new OkObjectResult($"{rows} rows inserted");
      }
      catch (Exception e)
      {
        log.LogError($"Error executing DeleteTransaction :: {e}");
        return new BadRequestResult();
      }
    }
  }
}
