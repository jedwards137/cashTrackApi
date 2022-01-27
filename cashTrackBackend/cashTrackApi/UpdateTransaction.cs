using cashTrackApi.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace cashTrackApi
{
  public static class UpdateTransaction
  {
    [FunctionName("UpdateTransaction")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "transactions/{tId}/azureId/{azureId}")] HttpRequest req, ILogger log,
        [CosmosDB(
          databaseName: "cashTrackDatabase",
          collectionName: "transactionsContainer",
          Id = "{azureId}",
          PartitionKey = "{tid}",
          ConnectionStringSetting = "cosmosDbConnection")] Document document,
        [CosmosDB(
          databaseName: "cashTrackDatabase",
          collectionName: "transactionsContainer",
          ConnectionStringSetting = "cosmosDbConnection")] DocumentClient client, string tId, string azureId)
    {
      var httpRequestBody = await req.GetBodyAsync<TransactionDto>();

      var isInvalidRequestBody = !httpRequestBody.IsValid;
      if (isInvalidRequestBody)
      {
        var bodyValidationErrorMessage = string.Join(", ", httpRequestBody.ValidationResults.Select(s => s.ErrorMessage).ToArray());
        log.LogError($"Error executing CreateTransaction :: {bodyValidationErrorMessage}");
        return new BadRequestResult();
      }

      var badRequest = document == null || string.IsNullOrEmpty(tId) || string.IsNullOrEmpty(azureId);
      if (badRequest)
      {
        return new BadRequestResult();
      }

      var updatesDto = httpRequestBody.Value;
      document.SetPropertyValue("Date", updatesDto.Date);
      document.SetPropertyValue("Description", updatesDto.Description);
      document.SetPropertyValue("Category", updatesDto.Category);
      document.SetPropertyValue("SubCategory", updatesDto.SubCategory);
      document.SetPropertyValue("Account", updatesDto.Account);
      document.SetPropertyValue("Amount", updatesDto.Amount);
      document.SetPropertyValue("Type", updatesDto.Type);

      await client.ReplaceDocumentAsync(document, new RequestOptions() { PartitionKey = new PartitionKey(tId) });   // DeleteDocumentAsync(document.SelfLink, new RequestOptions() { PartitionKey = new PartitionKey(tId) });
      return new OkResult();
    }
  }
}
