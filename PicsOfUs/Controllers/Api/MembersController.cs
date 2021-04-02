using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using PicsOfUs.Dtos;
using PicsOfUs.Models;

namespace PicsOfUs.Controllers.Api
{
    public class MembersController : ApiController
    {
        private ApplicationDbContext _context;

        public MembersController()
        {
            _context = new ApplicationDbContext();
        }

        // GET  /api/members
        public IEnumerable<MemberDto> GetMembers()
        {
            return _context.Members.ToList().Select(Mapper.Map<Member, MemberDto>);
        }

        // GET /api/members/1
        public MemberDto GeMember(int id)
        {
            var member = _context.Members.SingleOrDefault(m => m.Id == id);

            if (member == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            return Mapper.Map<Member, MemberDto>(member);
        }

        // POST /api/members
        [HttpPost]
        public MemberDto CreateMember(MemberDto memberDto)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var member = Mapper.Map<MemberDto, Member>(memberDto);
            _context.Members.Add(member);
            _context.SaveChanges();

            memberDto.Id = member.Id;

            return memberDto;
        }

        // PUT /api/member/1
        [HttpPut]
        public void UpdateMember(int id, MemberDto memberDto)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var memberInDb = _context.Members.SingleOrDefault(m => m.Id == id);

            if (memberInDb == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            Mapper.Map(memberDto, memberInDb);

            _context.SaveChanges();
        }

        // DELETE /api/member/1
        [HttpDelete]
        public void DeleteMember(int id)
        {
            var member = _context.Members.SingleOrDefault(m => m.Id == id);

            if (member == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            _context.Members.Remove(member);
            _context.SaveChanges();
        }
    }
}
