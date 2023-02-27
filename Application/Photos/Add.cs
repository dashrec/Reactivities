using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
  public class Add //add photo handler
  {
    public class Command : IRequest<Result<Photo>>  // return result which is gonna be a type of Photos
    {
      public IFormFile File { get; set; } // this parameter name File must match the key in postmen when it comes to testing
    }



    public class Handler : IRequestHandler<Command, Result<Photo>>
    {
      private readonly DataContext _context;
      private readonly IPhotoAccessor _photoAccessor;
      private readonly IUserAccessor _userAccessor;
      public Handler(DataContext context, IPhotoAccessor photoAccessor, IUserAccessor userAccessor) // to save photo url in to the database
      {
        _userAccessor = userAccessor;
        _photoAccessor = photoAccessor;
        _context = context;
      }

      public async Task<Result<Photo>> Handle(Command request, CancellationToken cancellationToken)
      {
        var user = await _context.Users.Include(p => p.Photos)
        .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());

        if (user == null) return null;

        var photoUploadResult = await _photoAccessor.AddPhoto(request.File);

        var photo = new Photo
        {
          Url = photoUploadResult.Url,
          Id = photoUploadResult.PublicId
        };
        if (!user.Photos.Any(x => x.IsMain)) photo.IsMain = true; // if there is not main then set to true

        user.Photos.Add(photo); // add newly created photo (url and Id) in users collection

        var result = await _context.SaveChangesAsync() > 0;

        if (result) return Result<Photo>.Success(photo); // pass back Result object

        return Result<Photo>.Failure("Problem adding photo");


      }
    }
  }
}


// Even though this is a command. We do need to return our photo to the client because it's going to contain both the Url and the publicId of the photo
