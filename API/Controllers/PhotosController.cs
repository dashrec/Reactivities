using Application.Core;
using Application.Photos;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  public class PhotosController : BaseApiController
  {
    [HttpPost]
    public async Task<IActionResult> Add([FromForm] Add.Command command)
    {
      return HandleResult(await Mediator.Send(command)); // command includes the file
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
      return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));

    }


    [HttpPost("{id}/setMain")]
    public async Task<IActionResult> SetMain(string id)
    {
      return HandleResult(await Mediator.Send(new SetMain.Command { Id = id }));
    }
  }
}


//FromForm:  we're going to need to give this in an atribute to tell our API controller where to find the file.