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
