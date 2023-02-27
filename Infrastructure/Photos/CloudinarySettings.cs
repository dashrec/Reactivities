namespace Infrastructure.Photos
{

// we'll be able to use our CloudinarySettings to get access to the values of the CloudName, the ApiKey and the ApiSecret where we need to use them
  public class CloudinarySettings 
  {
    public string CloudName { get; set; }

    public string ApiKey { get; set; }


    public string ApiSecret { get; set; }

  }
}


// after configure it in ap service extensions file to get from appsettings.json the credentials of Cloudinary
 