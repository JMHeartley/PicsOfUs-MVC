using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using PicsOfUs.Dtos;
using PicsOfUs.Models;

namespace PicsOfUs.App_Start
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            Mapper.CreateMap<Member, MemberDto>();
            Mapper.CreateMap<MemberDto, Member>();
            Mapper.CreateMap<Photo, PhotoDto>();
            Mapper.CreateMap<PhotoDto, Photo>();
        }
    }
}