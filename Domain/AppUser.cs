

using Microsoft.AspNetCore.Identity;

namespace Domain
{
  public class AppUser : IdentityUser
  {

    public string DisplayName { get; set; }

    public string Bio { get; set; }

    public ICollection<ActivityAttendee> Activities { get; set; }

    public ICollection<Photo> Photos { get; set; } // adds relation to photo
  }
}

// DisplayName, Bio will be added in migrations additionally