using Domain;

namespace Application.Profiles
{
  public class Profile
  {
    public string Username { get; set; }

    public string DisplayName { get; set; }
    public string Bio { get; set; }

    public string Image { get; set; }

    public bool Following { get; set; } //  We want to know if currently logged in User is following that particular user.
    public int FollowersCount { get; set; } // we need to map the result of this in to the profiles class
    public int FollowingCount { get; set; }

    public ICollection<Photo> Photos { get; set; }

  }
}

