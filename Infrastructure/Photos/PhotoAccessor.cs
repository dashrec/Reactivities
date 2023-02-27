using Application.Interfaces;
using Application.Photos;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure.Photos
{
  public class PhotoAccessor : IPhotoAccessor // so this class implements IPhotoAccessor interface and we got access on methods there
  {

    private readonly Cloudinary _cloudinary;
    public PhotoAccessor(IOptions<CloudinarySettings> config) // IOptions comes from Microsoft.Extensions.Options and we pass in CloudinarySettings as a type and we call it config
    {
      var account = new Account(
          config.Value.CloudName,  //should match keys in appsettings Cloudinary object
          config.Value.ApiKey,
          config.Value.ApiSecret
      );

      _cloudinary = new Cloudinary(account);
    }

    public async Task<PhotoUploadResult> AddPhoto(IFormFile file)
    {
      if (file.Length > 0)
      {
        await using var stream = file.OpenReadStream(); // using keyword means dispose resources are beeing used by OpenReadStream() method

        var uploadParams = new ImageUploadParams // comes from CloudinaryDotNet
        {
          File = new FileDescription(file.FileName, stream),
          Transformation = new Transformation().Height(500).Width(500).Crop("fill") // let Cloudinary transform this into a square. 
        };
        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error != null)
        {
          throw new Exception(uploadResult.Error.Message);
        }

        return new PhotoUploadResult
        {
          PublicId = uploadResult.PublicId,
          Url = uploadResult.SecureUrl.ToString()
        };
      }

      return null; // if do not have file just return null
    }

    public async Task<string> DeletePhoto(string publicId)
    {
      var deleteParams = new DeletionParams(publicId);
      var result = await _cloudinary.DestroyAsync(deleteParams);
      return result.Result == "ok" ? result.Result : null;
    }
  }
}


// IPhotoAccessor is an interface from application layer

// add this sertvice in applicationServiceextentions file in api project