using Application.Core;
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
    public class Query : IRequest<Result<List<Activity>>> { }

    public class Handler : IRequestHandler<Query, Result<List<Activity>>>
    {
      private readonly DataContext _context;


      public Handler(DataContext context) // constructor
      {

        _context = context;

      }

      public async Task<Result<List<Activity>>> Handle(Query request, CancellationToken cancellationToken) // returning Task, list of activities
      {

        return Result<List<Activity>>.Success(await _context.Activities.ToListAsync(cancellationToken));
      }
    }

  }
}