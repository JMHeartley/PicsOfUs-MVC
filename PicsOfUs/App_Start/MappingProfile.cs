using AutoMapper;
using PicsOfUs.Dtos;
using PicsOfUs.Models;

namespace PicsOfUs
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            Mapper.CreateMap<Member, MemberDto>();
            Mapper.CreateMap<MemberDto, Member>();

            Mapper.CreateMap<Member, MiniMemberDto>();

            Mapper.CreateMap<Photo, PhotoDto>();
            Mapper.CreateMap<PhotoDto, Photo>();
        }
    }
}