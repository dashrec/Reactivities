using Application.Photos;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
  public interface IPhotoAccessor
    {
        Task<PhotoUploadResult> AddPhoto(IFormFile file);
        Task<string>DeletePhoto(string publicId);
            
       
    }
}

// So these two methods are not going to touch our database at all. They're purely for using for uploading and deleting images from cloud.