//using System.ComponentModel.DataAnnotations;
namespace Domain
{
  public class Activity
  {
    public Guid Id { get; set; }
    // [Required]
    public string Title { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public string City { get; set; }
    public string Venue { get; set; }
    public bool IsCancelled { get; set; }

    public ICollection<ActivityAttendee> Attendees { get; set; } = new List<ActivityAttendee>();// this will add an empty attendee as a default in to activities table instead of being null. 
    public ICollection<Comment> Comments { get; set; } = new List <Comment>(); // type Comment name Comments
  }
}
// entity is like model
// we'll send it to a new list and we'll call it comment so that we initialize that list as well.

