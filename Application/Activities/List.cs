using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
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
    public class Query : IRequest<Result<List<ActivityDto>>> { }

    public class Handler : IRequestHandler<Query, Result<List<ActivityDto>>>
    {
      private readonly DataContext _context;

      private readonly IMapper _mapper;

      public Handler(DataContext context, IMapper mapper) // constructor
      {
        _mapper = mapper;

        _context = context;

      }

      public async Task<Result<List<ActivityDto>>> Handle(Query request, CancellationToken cancellationToken) // returning Task, list of activities
      {

        var activities = await _context.Activities
       /*    .Include(a => a.Attendees)
          .ThenInclude(u => u.AppUser) */

          .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider)
          .ToListAsync(cancellationToken);

     //   var activitiesToReturn = _mapper.Map<List<ActivityDto>>(activities);

        return Result<List<ActivityDto>>.Success(activities);
      }
    }

  }
}