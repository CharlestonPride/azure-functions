using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace ChsPride
{
  public static class GetDirectors
  {
    [FunctionName("directors")]
    public static IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
        [CosmosDB(databaseName: "ChsPride",
                collectionName: "Directors",
                ConnectionStringSetting = "CosmosDBConnection",
                PartitionKey = "chspride")]
            IEnumerable<Director> directors,
        ILogger log)
    {
      var executives = directors.Where(d => d.Executive && d.Active)
          .OrderBy(d => d.Order)
          .Select(d => new DirectorDto(d));

      var members = directors.Where(d => !d.Executive && d.Active)
          .OrderBy(d => d.DateElectedToBoard)
          .Select(d => new DirectorDto(d));

      return new OkObjectResult(new DirectorsCollection(executives, members));
    }
  }

  public class DirectorsCollection
  {
    public DirectorsCollection(IEnumerable<DirectorDto> executives, IEnumerable<DirectorDto> members)
    {
      Executives = executives;
      Members = members;
    }

    public IEnumerable<DirectorDto> Executives { get; set; }
    public IEnumerable<DirectorDto> Members { get; set; }
  }

  public class Director
  {
    public bool Active { get; set; } = false;
    public string Id { get; set; }
    public string EnvId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public string Title { get; set; }

    public bool Executive { get; set; } = false;

    public int? Order { get; set; }
    public DateTime DateElected { get; set; }

    public DateTime DateElectedToBoard { get; set; }
    public Pronouns Pronouns { get; set; }
    public string Bio { get; set; }
  }

  public class Pronouns
  {
    public string Subjective { get; set; }

    public string Objective { get; set; }

    public string Possessive { get; set; }

    public override string ToString()
    {
      return CultureInfo.CurrentCulture.TextInfo.ToTitleCase($"{Subjective}/{Objective}/{Possessive}");
    }
  }

  public class DirectorDto
  {
    public DirectorDto(Director director)
    {
      Id = director.Id;
      FirstName = director.FirstName;
      LastName = director.LastName;
      Title = director.Title;
      Executive = director.Executive;
      DateElected = director.DateElected.ToString("MMMM yyyy");
      DateElectedToBoard = director.DateElectedToBoard.ToString("MMMM yyyy");
      if (!string.IsNullOrWhiteSpace(director.Bio))
      {
        Bio = director.Bio.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
      }

      Pronouns = director.Pronouns.ToString();
    }

    public string Id { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string Title { get; }
    public bool Executive { get; }
    public string DateElected { get; }
    public string DateElectedToBoard { get; }
    public string Pronouns { get; set; }
    public string[] Bio { get; }
  }
}