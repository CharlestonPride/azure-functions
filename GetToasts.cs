using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace ChsPride
{
  public static class GetToasts
  {
    [FunctionName("toasts")]
    public static IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "toast/{partitionKey}")]
            HttpRequest req,
        [CosmosDB(databaseName: "ChsPride",
                collectionName: "Toasts",
                ConnectionStringSetting = "CosmosDBConnection",
                PartitionKey="{partitionKey}")]
            IEnumerable<Document> toasts,
        ILogger log)
    {
      return new OkObjectResult(toasts);
    }
  }
}