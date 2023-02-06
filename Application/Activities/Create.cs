
using Domain;
using MediatR;
using Persistence;

// this use cases are isolated from client
// import activity from domain project
namespace Application.Activities
{
  public class Create
  {
    public class Command : IRequest  // command returns no data
    {
      public Activity Activity { get; set; }
    }


    public class Handler : IRequestHandler<Command>
    {
      private readonly DataContext _context;
      public Handler(DataContext context)
      {
        _context = context;
      }
      // Handle is going to return an object to our API controller and our API controller is going to return the object with the HTP response.
      public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
      {
        _context.Activities.Add(request.Activity); // at this moment we don't touch a database, we are just adding activity in memory
        await _context.SaveChangesAsync();

        return Unit.Value;
        // this is equivalent to nothing. And it's just a way of us letting our API controller know that we finished 
      }
    }
  }
}

//we're adding an activity to our activities inside our context in memory. The only time that we make a change is when we go back and save changes