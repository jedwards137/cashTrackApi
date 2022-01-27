using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace cashTrackApi
{
  public static class DeleteTransactionById
  {
    [FunctionName("DeleteTransaction")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "transactions/{tId}")] HttpRequest req, ILogger log,
        [CosmosDB(
          databaseName: "cashTrackDatabase", 
          collectionName: "transactionsContainer", 
          Id = "{tId}",
          PartitionKey = "{tid}",
          ConnectionStringSetting = "cosmosDbConnection")] Document document,
        [CosmosDB(
          databaseName: "cashTrackDatabase",
          collectionName: "transactionsContainer",
          ConnectionStringSetting = "cosmosDbConnection")] DocumentClient client, string tId)
    {
      var badRequest = document == null || string.IsNullOrEmpty(tId);
      if (badRequest)
      {
        return new BadRequestResult();
      }
      await client.DeleteDocumentAsync(document.SelfLink, new RequestOptions() { PartitionKey = new PartitionKey(tId) });
      return new OkResult();
    }
  }
}
