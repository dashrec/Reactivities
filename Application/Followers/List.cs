using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Followers
{
  public class List
  {
    public class Query : IRequest<Result<List<Profiles.Profile>>>
    {
      public string Predicate { get; set; }
      public string Username { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<List<Profiles.Profile>>>
    {
      private readonly DataContext _context;

      private readonly IMapper _mapper;
      private readonly IUserAccessor _userAccessor;

      public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
      {
        _userAccessor = userAccessor;
        _context = context;
        _mapper = mapper;
      }

      public async Task<Result<List<Profiles.Profile>>> Handle(Query request, CancellationToken cancellationToken)
      {
        var profiles = new List<Profiles.Profile>();

        switch (request.Predicate)
        {
          //  In the case of followers, we're not interested in a target user. We're interested in the observer.
          case "followers": // get all followers List
            profiles = await _context.UserFollowings.Where(x => x.Target.UserName == request.Username).Select(u => u.Observer) // So our profiles now is going to be a list of profiles that are the observers.
                .ProjectTo<Profiles.Profile>(_mapper.ConfigurationProvider, new { currentUsername = _userAccessor.GetUsername()}).ToListAsync(); // it passes currentUsername to our configuration
            break;

          case "following": // List of targets in other words List of users the user follows
            profiles = await _context.UserFollowings.Where(x => x.Observer.UserName == request.Username).Select(u => u.Target)
            .ProjectTo<Profiles.Profile>(_mapper.ConfigurationProvider, new { currentUsername = _userAccessor.GetUsername()}).ToListAsync();
            break;
        }

        return Result<List<Profiles.Profile>>.Success(profiles);
      }
    }
  }
}

// So we need to return a list of profiles based on whether or not this is a list of users that's following a user or is being followed by a user.
