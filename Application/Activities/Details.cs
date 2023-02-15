using Application.Core;
using Domain;
using MediatR;
using Persistence;

// details handler that's going to take care of the logic for returning an individual activity.

namespace Application.Activities
{
  public class Details
  {
    public class Query : IRequest<Result<Activity>> // IRequest interface returning a single activity
    {
      public Guid Id { get; set; } // Now, this one is going to take a parameter because we need to specify what the id of the activity we want to retrieve.

    }

    //handler
    public class Handler : IRequestHandler<Query, Result<Activity>> // first arg is our Query and second its gonna return a single Activity
    {
      private readonly DataContext _context; //initialized field

      //constructor
      public Handler(DataContext context)
      {
        _context = context;

      }

      //interface
      public async Task<Result<Activity>> Handle(Query request, CancellationToken cancellationToken)
      {
        var activity = await _context.Activities.FindAsync(request.Id);
        return Result<Activity>.Success(activity);

      
      }
    }

  }
}