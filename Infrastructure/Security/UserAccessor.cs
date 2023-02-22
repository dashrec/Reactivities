using System.Security.Claims;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Security
{
    // So we've got an interface and technically a service to go and get our username
  public class UserAccessor : IUserAccessor
  {
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserAccessor(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;

    }
    public string GetUsername()
    {
      return _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name); // 
    }
  }
}

// Well, we're not inside the context of our API project and we need to access the HTTP context.
// And we can do that via this interface (IHttpContextAccessor) because our HTTP context contains our user objects, and from our user objects we can get access to the claims inside the token.