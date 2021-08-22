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
    public class PicsController : ApiController
    {
        private ApplicationDbContext _context;

        public PicsController()
        {
            _context = new ApplicationDbContext();
        }

        // GET  /api/pics
        public IHttpActionResult GetPics()
        {
            NLogger.GetInstance().Info($"User (id: {User.Identity.GetUserId()}) requesting all pics");
            return Ok(
                _context.Pics
                    .Include(p => p.Subjects)
                    .ToList()
                    .Select(Mapper.Map<Pic, PicDto>)
                );
        }

        // GET /api/pics/1
        public IHttpActionResult GetPic(int id)
        {
            var pic = _context.Pics
                .Include(p => p.Subjects)
                .Include(p => p.Lovers)
                .SingleOrDefault(p => p.Id == id);

            if (pic == null)
            {
                NLogger.GetInstance().Warning($"Bad GET request by user (id: {User.Identity.GetUserId()}) for pic (id: {id})");
                return NotFound();
            }

            pic.IsLoved = pic.Lovers.Select(u => u.Id)
                .Contains(User.Identity.GetUserId());

            NLogger.GetInstance().Info($"User (id: {User.Identity.GetUserId()}) requested pic (id: {pic.Id})");
            return Ok(Mapper.Map<Pic, PicDto>(pic));
        }

        // POST /api/pics
        [HttpPost]
        public IHttpActionResult CreatePic(PicDto picDto)
        {
            if (!ModelState.IsValid)
            {
                NLogger.GetInstance().Warning($"Bad POST request by user (id: {User.Identity.GetUserId()}");
                return BadRequest();
            }

            var pic = Mapper.Map<PicDto, Pic>(picDto);
            _context.Pics.Add(pic);
            _context.SaveChanges();

            picDto.Id = pic.Id;

            NLogger.GetInstance().Info($"User (id: {User.Identity.GetUserId()}) created pic (id: {pic.Id})");
            return Created(new Uri($"{Request.RequestUri}/api/pics/{picDto.Id}"), picDto);
        }

        // PUT /api/pic/1
        [HttpPut]
        public IHttpActionResult UpdatePic(int id, PicDto picDto)
        {
            if (!ModelState.IsValid)
            {
                NLogger.GetInstance().Warning($"Bad PUT request by user (id: {User.Identity.GetUserId()}) for pic (id: {id})");
                return BadRequest();
            }

            var picInDb = _context.Pics
                .SingleOrDefault(m => m.Id == id);

            if (picInDb == null)
            {
                NLogger.GetInstance().Warning($"Bad PUT request by user (id: {User.Identity.GetUserId()}), pic (id: {id}) not found");
                return NotFound();
            }

            Mapper.Map(picDto, picInDb);

            _context.SaveChanges();

            NLogger.GetInstance().Info($"User (id: {User.Identity.GetUserId()}) updated pic (id: {id})");
            return StatusCode(HttpStatusCode.NoContent);
        }

        // PATCH /api/pic/1
        [HttpPatch]
        public IHttpActionResult LovePic(int id, LovedPicDto lovedDto)
        {
            if (!ModelState.IsValid)
            {
                NLogger.GetInstance().Warning($"Bad PATCH request by user (id: {User.Identity.GetUserId()}) for pic (id: {id})");
                return BadRequest();
            }

            var picInDb = _context.Pics
                .Include(m => m.Lovers)
                .SingleOrDefault(m => m.Id == id);

            if (picInDb == null)
            {
                NLogger.GetInstance().Warning($"Bad PATCH request by user (id: {User.Identity.GetUserId()}), pic (id: {id}) not found");
                return NotFound();
            }

            var userId = User.Identity.GetUserId();

            if (lovedDto.IsLoved)
            {
                NLogger.GetInstance().Info($"User (id: {User.Identity.GetUserId()}) loved pic (id: {id})");
                picInDb.Lovers.Add(_context.Users.Single(u => u.Id == userId));
            }
            else
            {
                NLogger.GetInstance().Info($"User (id: {User.Identity.GetUserId()}) unloved pic (id: {id})");
                picInDb.Lovers.Remove(_context.Users.Single(u => u.Id == userId));
            }

            _context.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE /api/pics/1
        [HttpDelete]
        public IHttpActionResult DeletePic(int id)
        {
            var pic = _context.Pics.SingleOrDefault(p => p.Id == id);

            if (pic == null)
            {
                NLogger.GetInstance().Warning($"Bad DELETE request by user (id: {User.Identity.GetUserId()}), pic (id: {id}) not found");
                return NotFound();
            }

            _context.Pics.Remove(pic);
            _context.SaveChanges();

            NLogger.GetInstance().Info($"User (id: {User.Identity.GetUserId()}) deleted pic (id: {pic.Id})");
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
