using Application.Profiles;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  public class ProfilesController : BaseApiController
  {
    [HttpGet("{username}")] // 
    public async Task<IActionResult> GetProfile(string username)
    {
      return HandleResult(await Mediator.Send(new Details.Query { Username = username })); // command includes the file
    }

    [HttpGet("{username}/activities")] // taking username from root parameter
    public async Task<IActionResult> GetUserActivities(string username, string predicate) // get predicate from query string
    {
      return HandleResult(await Mediator.Send(new ListActivities.Query
      { Username = username, Predicate = predicate }));
    }

  }
}