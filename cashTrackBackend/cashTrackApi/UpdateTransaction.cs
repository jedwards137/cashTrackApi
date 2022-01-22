using cashTrackApi.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace cashTrackApi
{
  public static class UpdateTransaction
  {
    [FunctionName("UpdateTransaction")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "transactions/{tId}")] HttpRequest req, string tId, ILogger log)
    {
      var httpRequestBody = await req.GetBodyAsync<TransactionResource>();
      var isInvalidRequestBody = !httpRequestBody.IsValid;
      if (isInvalidRequestBody)
      {
        var bodyValidationErrorMessage = string.Join(", ", httpRequestBody.ValidationResults.Select(s => s.ErrorMessage).ToArray());
        return new BadRequestObjectResult($"Error executing CreateTransaction :: {bodyValidationErrorMessage}");
      }

      try
      {
        var transactionId = int.Parse(tId);
        var transaction = httpRequestBody.Value;
        var dbConnectionStr = Environment.GetEnvironmentVariable("sqldb_connection");
        var query = "exec [dbo].[updateTransaction] " +
          $"@transactionId={transactionId}, " +
          $"@date='{transaction.Date}'," +
          $"@description='{transaction.Description}'," +
          $"@category='{transaction.Category}'," +
          $"@account='{transaction.Account}'," +
          $"@amount={transaction.Amount}";

        using SqlConnection conn = new SqlConnection(dbConnectionStr);
        conn.Open();
        using SqlCommand cmd = new SqlCommand(query, conn);
        var rows = await cmd.ExecuteNonQueryAsync();
        return new OkObjectResult($"{rows} rows inserted");
      }
      catch (Exception e)
      {
        log.LogError($"Error executing UpdateTransaction :: {e}");
        return new BadRequestResult();
      }
    }
  }
}
