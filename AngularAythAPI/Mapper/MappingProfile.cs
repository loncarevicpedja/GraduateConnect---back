using AngularAythAPI.Models;
using AutoMapper;

namespace AngularAythAPI.Mapper
{
    public class MappingProfile : Profile
    {

        public MappingProfile() {

            CreateMap<Theme, Labor>();
            //.ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.StudentId))
            //.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            //.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            //.ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));


        }
    }
}
