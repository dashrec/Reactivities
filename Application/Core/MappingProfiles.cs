
using AutoMapper;
using Domain;

namespace Application.Core
{
  public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Activity, Activity>(); // CreateMap is gonna take a look inside of activity
        }
        
    }
}