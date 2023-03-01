namespace Application.Comments
{
  public class CommentDto
  {
    public int id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Body { get; set; }

    public string Username { get; set; }
    public string DisplayName { get; set; }

    public string Image { get; set; }
  }
}

// dto defines what we want to send  to client side endpoint 