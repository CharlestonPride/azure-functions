using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace ChsPride
{
  public static class GetSetting
  {
    [FunctionName("settings")]
    public static IActionResult Run(
      [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "settings/{partitionKey}/{id}")]
      HttpRequest req,
      [CosmosDB(databaseName: "ChsPride",
        collectionName: "Settings",
        ConnectionStringSetting = "CosmosDBConnection",
        Id = "{id}",
        PartitionKey = "{partitionKey}")]
      Document setting,
      string id,
      string partitionKey,
      ILogger log)
    {

      if (setting == null)
      {
        log.Log(LogLevel.Error, $"Unable to find setting {id} in {partitionKey}.");
        return new NotFoundResult();
      }
      return new OkObjectResult(setting);
    }
  }
}