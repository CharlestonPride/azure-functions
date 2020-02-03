using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ChsPride
{
  public static class GetDirectors
  {
    [FunctionName("directors")]
    public static IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
        [CosmosDB(databaseName: "ChsPride",
                collectionName: "Directors",
                ConnectionStringSetting = "CosmosDBConnection",
                PartitionKey = "chspride")] IEnumerable<Director> directors,
            ILogger log)
    {

      foreach (var director in directors)
      {
        try
        {
          director.Elected = director.DateElected.ToString("MMMM yyyy");
          director.ElectedToBoard = director.DateElectedToBoard.ToString("MMMM yyyy");
          if (string.IsNullOrWhiteSpace(director.Bio))
            continue;
          director.BioParagraphs = director.Bio.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }
        catch (Exception ex)
        {
          log.LogError(ex, ex.Message);
        }
      }

      var directorsCollection = new DirectorsCollection(
        directors.Where(d => d.Executive).OrderBy(d => d.Order),
        directors.Where(d => !d.Executive).OrderBy(d => d.DateElectedToBoard));

      return new OkObjectResult(directorsCollection);
    }
  }

  public class DirectorsCollection
  {
    public DirectorsCollection(IEnumerable<Director> executives, IEnumerable<Director> members)
    {
      Executives = executives;
      Members = members;
    }
    public IEnumerable<Director> Executives { get; set; }
    public IEnumerable<Director> Members { get; set; }
  }
  public class Director
  {
    public string Id { get; set; }
    public string EnvId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public string Title { get; set; }

    public bool Executive { get; set; } = false;

    public int? Order { get; set; }
    public DateTime DateElected { get; set; }

    public DateTime DateElectedToBoard { get; set; }
    public string Elected { get; set; }

    public string ElectedToBoard { get; set; }

    public Pronouns Pronouns { get; set; }
    public string Bio { get; set; }

    public string[] BioParagraphs { get; set; }
  }

  public class Pronouns
  {
    public string Subjective { get; set; }

    public string Objective { get; set; }

    public string Possessive { get; set; }
  }
}
