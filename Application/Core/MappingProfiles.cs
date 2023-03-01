
using Application.Activities;
using Application.Comments;
using AutoMapper;
using Domain;

namespace Application.Core
{
  public class MappingProfiles : Profile
  {
    public MappingProfiles()
    {
      CreateMap<Activity, Activity>(); // CreateMap is gonna take a look inside of activity
      CreateMap<Activity, ActivityDto>().ForMember(d => d.HostUsername, o => o.MapFrom(s => s.Attendees.FirstOrDefault(x => x.IsHost).AppUser.UserName));
      CreateMap<ActivityAttendee, AttendeeDto>()
       .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.AppUser.DisplayName))
       .ForMember(d => d.Username, o => o.MapFrom(s => s.AppUser.UserName))
       .ForMember(d => d.Bio, o => o.MapFrom(s => s.AppUser.Bio))
       .ForMember(d => d.Image, o => o.MapFrom(s => s.AppUser.Photos.FirstOrDefault(x => x.IsMain).Url));

      // we go from  user to profile and update set main photo
      CreateMap<AppUser, Profiles.Profile>().ForMember(d => d.Image, o => o.MapFrom(s => s.Photos.FirstOrDefault(x => x.IsMain).Url));
      CreateMap<Comment, CommentDto>()
       .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.Author.DisplayName)) // so we get DisplayName from related author of the comment
       .ForMember(d => d.Username, o => o.MapFrom(s => s.Author.UserName))
       .ForMember(d => d.Image, o => o.MapFrom(s => s.Author.Photos.FirstOrDefault(x => x.IsMain).Url));
    }

  }
}

// we need DisplayName, Username, Image to send to client side endpoint. therefore we go to Comment entity, then to Author because it's a related property, get those properties and then populate them in CommentDto
// we are gonna create new map There's going to go from our comment to our commentDTA. 
// create 2 handlers 1 to create comment and the second to list the comments for specific activity