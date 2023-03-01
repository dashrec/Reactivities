namespace Domain
{
  public class Comment
  {
    public int Id { get; set; }

    public string Body { get; set; }
    public AppUser Author { get; set; } // related property
    public Activity Activity { get; set; } // related activity

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  }
}

// after  that add relation to activity  for comments