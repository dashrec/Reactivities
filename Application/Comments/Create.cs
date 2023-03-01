using Application.Core;
using Application.Interfaces;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

// comment create handler
namespace Application.Comments
{
  public class Create
  {
    public class Command : IRequest<Result<CommentDto>> // IRequest interface
    {
      public string Body { get; set; }
      public Guid ActivityId { get; set; }
    }

    public class CommandValidator : AbstractValidator<Command>
    {

      public CommandValidator() // constructor 
      {
        RuleFor(x => x.Body).NotEmpty();
      }
    }

    //  even though this is a command, we are going to be returning from this because we need our server to generate the ID for our comments, 
    // and that's not something that we can do from the client side.
    // we want to get the user properties that shapes the comment data . So this is the reason that we're returning A CommentDto in this case.

    public class Handler : IRequestHandler<Command, Result<CommentDto>>
    {
      private readonly DataContext _context;
      private readonly IMapper _mapper;
      private readonly IUserAccessor _userAccessor;

      // we need DataContext to update database with new comment. we need IMapper to shape the data we ar returning and IUserAccessor to get current user
      public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor) 
      {
        _userAccessor = userAccessor;
        _mapper = mapper;
        _context = context;
      }

      public async Task<Result<CommentDto>> Handle(Command request, CancellationToken cancellationToken)
      {
        var activity = await _context.Activities // get activity
                   .Include(x => x.Comments)
                   .ThenInclude(x => x.Author)
                   .ThenInclude(x => x.Photos)
                   .FirstOrDefaultAsync(x => x.Id == request.ActivityId);


        if (activity == null) return null;

        var user = await _context.Users
            .SingleOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());

        var comment = new Comment
        {
          Author = user,
          Activity = activity,
          Body = request.Body
        };

        activity.Comments.Add(comment);

        var success = await _context.SaveChangesAsync() > 0;

        if (success) return Result<CommentDto>.Success(_mapper.Map<CommentDto>(comment));

        return Result<CommentDto>.Failure("Failed to add comment");
      }
    }

  }
}