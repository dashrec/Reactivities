namespace Domain
{
  public class UserFollowing
  {
    public string ObserverId { get; set; }
    public AppUser Observer { get; set; } // person who is going to follow
    public string TargetId { get; set; }
    public AppUser Target { get; set; } // person who is going to be followed
  }
}

// join entity
// then add this relationship to AppUser entity
// so each time we create a following, we're just going to have an entry in UserFollowings table
// and I use the UserFollowings for the ObserverId and the TargetId.
// ObserverId and TargetId will be  foreign keys in users table 