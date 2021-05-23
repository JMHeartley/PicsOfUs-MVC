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
            Mapper.CreateMap<Member, MiniMemberDto>();
            Mapper.CreateMap<MemberDto, Member>();

            Mapper.CreateMap<Pic, PicDto>();
            Mapper.CreateMap<PicDto, Pic>();
            Mapper.CreateMap<LovedPicDto, Pic>();
        }
    }
}