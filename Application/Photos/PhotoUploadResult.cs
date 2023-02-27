using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Photos
{
  public class PhotoUploadResult
  {
    public string PublicId { get; set; }

    public string Url { get; set; }
  }
}
// our application does not have excess to infrastructure project to access upload result we got from cloudinary so we create this class

// we get back PublicId and url of image from this class