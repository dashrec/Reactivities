using Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Activities;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
// our controllers are going to receive an HTTP request from a client and controller is going to send this request to application layer
// it's going to respond with an HTP response, and that's what is going to pass back out to the client.
// When a request comes to our API controller, we're going to send something using the Mediator to send method and we can even make a query or send a command via the mediator.
// We have API controllers and these are going to retrieve the information from our application layer.
// it derives from BaseApiController
{
  public class ActivitiesController : BaseApiController
  {
    [HttpGet] //api/activities
    public async Task<IActionResult> GetActivities()
    {
      return HandleResult(await Mediator.Send(new List.Query())); // it returns all. this Mediator derives from base class controller
    }

    //[Authorize]
    [HttpGet("{id}")] //api/activities/someId
    public async Task<IActionResult> GetActivity(Guid id) // this id gonna match this [HttpGet("{id}")] so this id /someId
    {


      return HandleResult(await Mediator.Send(new Details.Query { Id = id }));

      /*   var activity = await Mediator.Send(new Details.Query { Id = id }); // it returns only one record with appropriate id
        if (activity == null) return NotFound(); // it turns 204 null response from db to 404 not found
        return activity; */
    }


    [HttpPost]
    public async Task<IActionResult> CreateActivity(Activity activity) // activity object
    {
      return HandleResult(await Mediator.Send(new Create.Command { Activity = activity }));
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> EditActivity(Guid id, Activity activity) // get activity from the body of request
    {
      activity.Id = id;
      return HandleResult(await Mediator.Send(new Edit.Command { Activity = activity })); // send back to a client
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteActivity(Guid id)
    {
      return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));
    }

  }
}


// And when we use our IActionResult, it gives us access to the response such as return, OK, return, bad request, return not found.