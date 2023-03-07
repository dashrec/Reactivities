using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles
{
  public class ListActivities
  {
    public class Query : IRequest<Result<List<UserActivityDto>>>
    {
      public string Username { get; set; }

      //Predicate to say whether or not we're looking at the past events, the future events or the events the user is hosting.
      public string Predicate { get; set; } 
    }

    public class Handler : IRequestHandler<Query, Result<List<UserActivityDto>>>
    {
      private readonly DataContext _context;
      private readonly IMapper _mapper;
      public Handler(DataContext context, IMapper mapper)
      {
        _mapper = mapper;
        _context = context;
      }

      public async Task<Result<List<UserActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
      {
        var query = _context.ActivityAttendees
            .Where(u => u.AppUser.UserName == request.Username)
            .OrderBy(a => a.Activity.Date)
            .ProjectTo<UserActivityDto>(_mapper.ConfigurationProvider) // go from ActivityAttendees to UserActivityDto
            .AsQueryable(); // So we're not executing anything to the database yet.

        var today = DateTime.UtcNow;

        query = request.Predicate switch
        {
          "past" => query.Where(a => a.Date <= today),
          "hosting" => query.Where(a => a.HostUsername == request.Username),
          _ => query.Where(a => a.Date >= today) // default case
        };

        var activities = await query.ToListAsync();

        return Result<List<UserActivityDto>>.Success(activities);
      }
    }
  }
}