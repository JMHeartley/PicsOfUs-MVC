using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using PicsOfUs.Dtos;
using PicsOfUs.Models;
using System.Data.Entity;

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
        public IHttpActionResult GetMembers()
        {
            return Ok(
                _context.Members
                    .Include(m => m.Siblings)
                    .Include(m => m.Parents)
                    .Include(m => m.Children)
                    .ToList()
                    .Select(Mapper.Map<Member, MemberDto>)
                );
        }

        // GET /api/members/1
        public IHttpActionResult GetMember(int id)
        {
            var member = _context.Members
                .Include(m => m.Siblings)
                .Include(m => m.Parents)
                .Include(m => m.Children)
                .SingleOrDefault(m => m.Id == id);

            if (member == null)
            {
                return BadRequest();
            }

            return Ok(Mapper.Map<Member, MemberDto>(member));
        }

        // POST /api/members
        [HttpPost]
        public IHttpActionResult CreateMember(MemberDto memberDto)
        {
            if (!ModelState.IsValid)
            {
                BadRequest();
            }

            var member = Mapper.Map<MemberDto, Member>(memberDto);
            _context.Members.Add(member);
            _context.SaveChanges();

            memberDto.Id = member.Id;

            return Created(new Uri(Request.RequestUri + "/api/members/" + memberDto.Id), memberDto);
        }

        // PUT /api/member/1
        [HttpPut]
        public IHttpActionResult UpdateMember(int id, MemberDto memberDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var memberInDb = _context.Members
                .Include(m => m.Siblings)
                .Include(m => m.Parents)
                .Include(m => m.Children)
                .SingleOrDefault(m => m.Id == id);

            if (memberInDb == null)
            {
                return NotFound();
            }

            Mapper.Map(memberDto, memberInDb);

            _context.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE /api/member/1
        [HttpDelete]
        public IHttpActionResult DeleteMember(int id)
        {
            var member = _context.Members
                .Include(m => m.Siblings)
                .Include(m => m.Parents)
                .Include(m => m.Children)
                .SingleOrDefault(m => m.Id == id);

            if (member == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            _context.Members.Remove(member);
            _context.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
