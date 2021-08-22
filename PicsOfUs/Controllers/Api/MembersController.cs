using AutoMapper;
using Microsoft.AspNet.Identity;
using PicsOfUs.Dtos;
using PicsOfUs.Models;
using PicsOfUs.Utilities;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Http;

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
            NLogger.GetInstance().Info($"User (id: {User.Identity.GetUserId()}) requesting all members");
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
                NLogger.GetInstance().Warning($"Bad GET request by user (id: {User.Identity.GetUserId()}), member (id: {id}) not found");
                return NotFound();
            }

            NLogger.GetInstance().Info($"User (id: {User.Identity.GetUserId()}) requested member (id: {member.Id})");
            return Ok(Mapper.Map<Member, MemberDto>(member));
        }

        // POST /api/members
        [HttpPost]
        public IHttpActionResult CreateMember(MemberDto memberDto)
        {
            if (!ModelState.IsValid)
            {
                NLogger.GetInstance().Warning($"Bad POST request by user (id: {User.Identity.GetUserId()}");
                return BadRequest();
            }

            var member = Mapper.Map<MemberDto, Member>(memberDto);
            _context.Members.Add(member);
            _context.SaveChanges();

            memberDto.Id = member.Id;

            NLogger.GetInstance().Info($"User (id: {User.Identity.GetUserId()}) created member (id: {member.Id})");
            return Created(new Uri($"{Request.RequestUri}/api/members/{memberDto.Id}"), memberDto);
        }

        // PUT /api/member/1
        [HttpPut]
        public IHttpActionResult UpdateMember(int id, MemberDto memberDto)
        {
            if (!ModelState.IsValid)
            {
                NLogger.GetInstance().Warning($"Bad PUT request by user (id: {User.Identity.GetUserId()}) for member (id: {id})");
                return BadRequest();
            }

            var memberInDb = _context.Members
                .Include(m => m.Siblings)
                .Include(m => m.Parents)
                .Include(m => m.Children)
                .SingleOrDefault(m => m.Id == id);

            if (memberInDb == null)
            {
                NLogger.GetInstance().Warning($"Bad PUT request by user (id: {User.Identity.GetUserId()}), member (id: {id}) not found");
                return NotFound();
            }

            Mapper.Map(memberDto, memberInDb);

            _context.SaveChanges();

            NLogger.GetInstance().Info($"User (id: {User.Identity.GetUserId()}) updated member (id: {id})");
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
                NLogger.GetInstance().Warning($"Bad DELETE request by user (id: {User.Identity.GetUserId()}), member (id: {id}) not found");
                NotFound();
            }

            _context.Members.Remove(member);
            _context.SaveChanges();

            NLogger.GetInstance().Info($"User (id: {User.Identity.GetUserId()}) deleted member (id: {member.Id})");
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
