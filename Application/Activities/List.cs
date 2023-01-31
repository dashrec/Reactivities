using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

// mediator query

namespace Application.Activities
{
  public class List
  {
    public class Query : IRequest<List<Activity>> { }

    public class Handler : IRequestHandler<Query, List<Activity>>
    {
      private readonly DataContext _context;
    

      public Handler(DataContext context) // constructor
      {
       
        _context = context;

      }

      public async Task<List<Activity>> Handle(Query request, CancellationToken token) // returning Task, list of activities
      {
      
        return await _context.Activities.ToListAsync();
      }
    }

  }
}