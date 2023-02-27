namespace Domain
{
  public class Photo
  {
    public string Id { get; set; } // we get back from cloudinary

    public string Url { get; set; }
    public bool IsMain { get; set; }
  }
}


