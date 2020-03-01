using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace ChsPride
{
  public static class GetAdvertisements
  {
    //Note the route here can not be called advertisements. Chrome will block it
    [FunctionName("advertisements")]
    public static IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "guide/{partitionKey}")]
            HttpRequest req,
        [CosmosDB(databaseName: "ChsPride",
                collectionName: "Advertisements",
                ConnectionStringSetting = "CosmosDBConnection",
                SqlQuery = "SELECT * FROM c WHERE c.envId = {partitionKey} ORDER BY c.rate DESC")]
            IEnumerable<Document> advertisements,
        ILogger log)
    {
      return new OkObjectResult(advertisements);
    }
  }
}