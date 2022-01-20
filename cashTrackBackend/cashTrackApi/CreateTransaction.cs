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
  public static class CreateTransaction
  {
    [FunctionName("CreateTransaction")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "transactions")] HttpRequest req, ILogger log)
    {
      ValidationWrapper<TransactionResource> httpRequestBody = await req.GetBodyAsync<TransactionResource>();

      var isInvalidRequestBody = !httpRequestBody.IsValid;
      if (isInvalidRequestBody)
      {
        var bodyValidationErrorMessage = string.Join(", ", httpRequestBody.ValidationResults.Select(s => s.ErrorMessage).ToArray());
        return new BadRequestObjectResult($"Invalid Input: {bodyValidationErrorMessage}");
      }

      var dbConnectionStr = Environment.GetEnvironmentVariable("sqldb_connection");
      var transaction = httpRequestBody.Value;
      var query = "exec [dbo].[createTransaction] " +
        $"@date = '{transaction.Date}'," +
        $"@description = '{transaction.Description}'," +
        $"@category = '{transaction.Category}'," +
        $"@account = '{transaction.Account}'," +
        $"@amount = {transaction.Amount}";

      try
      {
        using SqlConnection conn = new SqlConnection(dbConnectionStr);
        conn.Open();
        using SqlCommand cmd = new SqlCommand(query, conn);
        var rows = await cmd.ExecuteNonQueryAsync();
        return new OkObjectResult($"{rows} rows inserted");
      }
      catch (Exception e)
      {
        log.LogError(e.ToString());
        return new BadRequestResult();
      }
    }
  }
}
