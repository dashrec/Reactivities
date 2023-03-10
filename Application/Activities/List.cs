using System.Linq;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

// mediator query

namespace Application.Activities
{
  public class List
  {
    public class Query : IRequest<Result<PagedList<ActivityDto>>>
    {

      public ActivityParams Params { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<PagedList<ActivityDto>>>
    {
      private readonly DataContext _context;

      private readonly IMapper _mapper;
      private readonly IUserAccessor _userAccessor;

      public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor) // constructor
      {
        _userAccessor = userAccessor;
        _mapper = mapper;

        _context = context;

      }

      public async Task<Result<PagedList<ActivityDto>>> Handle(Query request, CancellationToken cancellationToken) // returning Task, list of activities
      {
        var query = _context.Activities
          .Where(x => x.Date >= request.Params.StartDate) // filtering by date.
          .OrderBy(d => d.Date)
          .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider, new { currentUsername = _userAccessor.GetUsername() })
          .AsQueryable();


        if (request.Params.IsGoing && !request.Params.IsHost)
        {
          query = query.Where(x => x.Attendees.Any(a => a.Username == _userAccessor.GetUsername())); // where im going
        }

        if (request.Params.IsHost && !request.Params.IsGoing)
        {
          query = query.Where(x => x.HostUsername == _userAccessor.GetUsername()); // where im posting
        }


        return Result<PagedList<ActivityDto>>.Success(
          await PagedList<ActivityDto>.CreateAsync(query, request.Params.PageNumber, request.Params.PageSize)
        );
      }
    }

  }
}