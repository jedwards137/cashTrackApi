using cashTrackApi.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace cashTrackApi
{
  public static class CreateTransaction
  {
    [FunctionName("CreateTransaction")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "transactions")] HttpRequest req, ILogger log,
        [CosmosDB(
          databaseName: "cashTrackDatabase",
          collectionName: "transactionsContainer",
          ConnectionStringSetting = "cosmosDbConnection")] IAsyncCollector<object> transactions)
    {
      var httpRequestBody = await req.GetBodyAsync<TransactionDto>();

      var isInvalidRequestBody = !httpRequestBody.IsValid;
      if (isInvalidRequestBody)
      {
        var bodyValidationErrorMessage = string.Join(", ", httpRequestBody.ValidationResults.Select(s => s.ErrorMessage).ToArray());
        log.LogError($"Error executing CreateTransaction :: {bodyValidationErrorMessage}");
        return new BadRequestResult();
      }

      try
      {
        await transactions.AddAsync(httpRequestBody.Value);
        return new OkObjectResult("");
      }
      catch (Exception e)
      {
        log.LogError($"Error executing CreateTransaction :: {e}");
        return new BadRequestResult();
      }
    }
  }
}
