using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Travel_Agency_Server.DTO;
using Travel_Agency_Server.Model;

namespace Travel_Agency_Server.Helpers
{
    public class MappingProfiles : Profile
    {

        public MappingProfiles()
        {
            CreateMap<Trip, TripToReturnDTO>().ForMember(d => d.Image, o => o.MapFrom<TripImageUrlResolver>()); ;
            CreateMap<Trip, TripDTO>().ReverseMap();
            CreateMap<IdentityUser, UserToReturnDTO>();
        }
    }
}
