

using Microsoft.AspNetCore.Identity;

namespace Domain
{
  public class AppUser : IdentityUser
  {

    public string DisplayName { get; set; }

    public string Bio { get; set; }

    public ICollection<ActivityAttendee> Activities { get; set; }

    public ICollection<Photo> Photos { get; set; } // adds relation to photo

    public ICollection<UserFollowing> Followings { get; set; } // Who is the current user following 
    public ICollection<UserFollowing> Followers { get; set; } // Who is following the currently logged in user?
  }
}

