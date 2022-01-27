using cashTrackApi.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace cashTrackApi
{
  public static class GetTransactionsByYearAndMonth
  {
    [FunctionName("GetTransactionsByYearAndMonth")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "transactions/year/{yearId}/month/{monthId}")] HttpRequest req, ILogger log,
        [CosmosDB(
          databaseName: "cashTrackDatabase",
          collectionName: "transactionsContainer",
          ConnectionStringSetting = "cosmosDbConnection",
          SqlQuery = "SELECT * FROM c WHERE DateTimePart('m', c.Date) = {monthId} AND DateTimePart('yy', c.Date) = {yearId}")] 
            IEnumerable<TransactionResource> transactionItems, 
        int yearId, int monthId)
    {
      return new OkObjectResult(transactionItems);
    }
  }
}
