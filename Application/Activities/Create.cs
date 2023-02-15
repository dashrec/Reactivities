
using Application.Core;
using Domain;
using FluentValidation;
using MediatR;
using Persistence;


// this use cases are isolated from client
// import activity from domain project
namespace Application.Activities
{
  public class Create
  {
    public class Command : IRequest<Result<Unit>>  // command returns no data. Unit we are not returning anything
    {
      public Activity Activity { get; set; }
    }

    // validation
    public class CommandValidator : AbstractValidator<Command>
    {
      public CommandValidator()
      {
        RuleFor(x => x.Activity).SetValidator(new ActivityValidator());
      }
    }


    public class Handler : IRequestHandler<Command, Result<Unit>>
    {
      private readonly DataContext _context;
      public Handler(DataContext context)
      {
        _context = context;
      }
      // Handle is going to return an object to our API controller and our API controller is going to return the object with the HTtP response.
      public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
      {
        _context.Activities.Add(request.Activity);
        var result = await _context.SaveChangesAsync() > 0;
        if (!result) return Result<Unit>.Failure("Failed to create activity");
        return Result<Unit>.Success(Unit.Value);

      }
    }
  }
}

//we're adding an activity to our activities inside our context in memory. The only time that we make a change is when we go back and save changes