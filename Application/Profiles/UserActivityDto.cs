using System.Text.Json.Serialization;

namespace Application.Profiles
{
  public class UserActivityDto
  {
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Category { get; set; }
    public DateTime Date { get; set; }

    [JsonIgnore]
    public string HostUsername { get; set; }
  }
}

// if we want a property that we add to our class to help us out, but we don't actually
// want to return to the client, we can add an attribute called JsonIgnore.