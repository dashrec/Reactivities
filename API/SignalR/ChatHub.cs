using Application.Comments;
using MediatR;
using Microsoft.AspNetCore.SignalR;


// instead of  using api controllers we use signalR as the endpoints that the clients connect to.
namespace API.SignalR
{
    public class ChatHub : Hub
    {
        private readonly IMediator _mediator;

        public ChatHub(IMediator mediator)
        {
            _mediator = mediator;
        }

        //  our client will make a connection to this hub and then it will be able to invoke methods that we create inside this hub.
        public async Task SendComment(Create.Command command)  // Create is a class in Application.Comments Create.es file that saves comments in db
        {      
          // saved comment result   
            var comment = await _mediator.Send(command); // because comment is a result object that contains commentDto we need to get the value of that comment

            // clients object has access to connected clients 
            // command is going to have the commentID and it's going to be shaped via auto mapper into a commentDTO and we want to send that to anybody who is connected to 
            // the hub, including the person that's made the comment in the first place.
            await Clients.Group(command.ActivityId.ToString()) // each activity is gonna have a group. Because ActivityId is Guid we need to make it string
                .SendAsync("ReceiveComment", comment.Value); // we will use the name "ReceiveComment" in the client side 
        }


      // override OnConnectedAsync methods inside of the hub
        public override async Task OnConnectedAsync() // do something when client is connected
        {
            var httpContext = Context.GetHttpContext();
            var activityId = httpContext.Request.Query["activityId"]; // make sure that we send up a query string from the client that contains the activityId as key
            await Groups.AddToGroupAsync(Context.ConnectionId, activityId); // add connected client to a particular groupe with activityId
            var result = await _mediator.Send(new List.Query{ActivityId = Guid.Parse(activityId)}); // send list of comments down to the clients
            await Clients.Caller.SendAsync("LoadComments", result.Value); // The only user we need to send this to is the caller, the person making this request to connect to our signal, our hub.
        }
    }
}