using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace ChsPride
{
  public static class GetSponsorships
  {
    [FunctionName("sponsorships")]
    public static IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "sponsorships/{partitionKey}")]
            HttpRequest req,
        [CosmosDB(databaseName: "ChsPride",
                collectionName: "Sponsorships",
                ConnectionStringSetting = "CosmosDBConnection",
                SqlQuery = "SELECT * FROM c WHERE c.envId = {partitionKey} ORDER BY c.rate DESC")]
            IEnumerable<Document> sponsorships,
        ILogger log)
    {
      return new OkObjectResult(sponsorships);
    }
  }
}