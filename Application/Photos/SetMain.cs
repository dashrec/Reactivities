using Application.Core;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
  public class SetMain
  {
    public class Command : IRequest<Result<Unit>>
    {
      public string Id { get; set; }
    }

    public class Handler : IRequestHandler<Command, Result<Unit>>
    {
      private readonly DataContext _context;
      private readonly IUserAccessor _userAccessor;
      public Handler(DataContext context, IUserAccessor userAccessor)
      {
        _userAccessor = userAccessor;
        _context = context;
      }

      public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
      {
        var user = await _context.Users.Include(p => p.Photos)// Include user photo collection
        .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername()); // get user object 
        if (user == null) return null;

        var photo = user.Photos.FirstOrDefault(x => x.Id == request.Id); // get the photo we want to set as main

        if (photo == null) return null;

        var currentMain = user.Photos.FirstOrDefault(x => x.IsMain); // get main photo
        if (currentMain != null) currentMain.IsMain = false;
        photo.IsMain = true; // set the one we want to make main to true
        var success = await _context.SaveChangesAsync() > 0;
        if (success) return Result<Unit>.Success(Unit.Value);
        return Result<Unit>.Failure("Problem setting main photo");

      }
    }
  }
}