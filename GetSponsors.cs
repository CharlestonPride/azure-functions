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
  public static class GetSponsors
  {
    [FunctionName("sponsors")]
    public static IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "sponsors/{partitionKey}")]
            HttpRequest req,
        [CosmosDB(databaseName: "ChsPride",
                collectionName: "Sponsors",
                ConnectionStringSetting = "CosmosDBConnection",
                SqlQuery = "SELECT * FROM c WHERE c.envId = {partitionKey}")]
            IEnumerable<Document> sponsors,
        string partitionKey,
        ILogger log)
    {
      var sponsorsList = sponsors.ToList();

      if (ShouldShuffle(req))
      {
        sponsorsList.Shuffle();
      }

      if (ShouldGroup(req, partitionKey))
      {
        var grouped = GroupSponsors(sponsorsList);
        return new OkObjectResult(grouped);
      }

      return new OkObjectResult(sponsorsList);
    }

    private static bool ShouldShuffle(HttpRequest req)
    {
      return req.Query.ContainsKey("shuffle") && bool.TryParse(req.Query["shuffle"][0], out var shouldShuffle) && shouldShuffle;
    }

    private static GroupedSponsors GroupSponsors(List<Document> sponsorsList)
    {
      var grouped = new GroupedSponsors();
      foreach (var sponsor in sponsorsList)
      {
        var level = sponsor.GetPropertyValue<string>("level");
        switch (level)
        {
          case "red":
            grouped.Red.Add(sponsor);
            break;
          case "orange":
            grouped.Orange.Add(sponsor);
            break;
          case "yellow":
            grouped.Yellow.Add(sponsor);
            break;
          case "green":
            grouped.Green.Add(sponsor);
            break;
          case "blue":
            grouped.Blue.Add(sponsor);
            break;
          case "purple":
            grouped.Purple.Add(sponsor);
            break;
        }
      }

      return grouped;
    }

    private static bool ShouldGroup(HttpRequest req, string partitionKey)
    {
      return partitionKey == "chspride" && req.Query.ContainsKey("group") && bool.TryParse(req.Query["group"][0], out var shouldGroup) && shouldGroup;
    }

    private static readonly Random Rng = new Random();

    private static void Shuffle<T>(this IList<T> list)
    {
      int n = list.Count;
      while (n > 1)
      {
        n--;
        int k = Rng.Next(n + 1);
        T value = list[k];
        list[k] = list[n];
        list[n] = value;
      }
    }

    public class GroupedSponsors
    {
      public List<Document> Red { get; set; } = new List<Document>();
      public List<Document> Orange { get; set; } = new List<Document>();
      public List<Document> Yellow { get; set; } = new List<Document>();
      public List<Document> Green { get; set; } = new List<Document>();
      public List<Document> Blue { get; set; } = new List<Document>();
      public List<Document> Purple { get; set; } = new List<Document>();

    }
  }
}