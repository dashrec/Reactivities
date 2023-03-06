using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

// details handler that's going to take care of the logic for returning an individual activity.

namespace Application.Activities
{
  public class Details
  {
    public class Query : IRequest<Result<ActivityDto>> // IRequest interface returning a single activity
    {
      public Guid Id { get; set; } // Now, this one is going to take a parameter because we need to specify what the id of the activity we want to retrieve.

    }

    //handler
    public class Handler : IRequestHandler<Query, Result<ActivityDto>> // first arg is our Query and second its gonna return a single Activity
    {
      private readonly DataContext _context; //initialized field
      private readonly IMapper _mapper;
      private readonly IUserAccessor _userAccessor;

      //constructor
      public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
      {
        _userAccessor = userAccessor;
        _mapper = mapper;
        _context = context;

      }

      //interface
      public async Task<Result<ActivityDto>> Handle(Query request, CancellationToken cancellationToken)
      {
        var activity = await _context.Activities
        .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider, new { currentUsername = _userAccessor.GetUsername() })
        .FirstOrDefaultAsync(x => x.Id == request.Id);


        return Result<ActivityDto>.Success(activity);


      }
    }

  }
}