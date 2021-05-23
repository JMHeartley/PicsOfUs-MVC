using AutoMapper;
using PicsOfUs.Dtos;
using PicsOfUs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using System.Net.Configuration;
using Microsoft.AspNet.Identity;

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
                return BadRequest();
            }

            pic.IsLoved = pic.Lovers.Select(u => u.Id)
                .Contains(User.Identity.GetUserId());

            return Ok(Mapper.Map<Pic, PicDto>(pic));
        }

        // POST /api/pics
        [HttpPost]
        public IHttpActionResult CreatePic(PicDto picDto)
        {
            if (!ModelState.IsValid)
            {
                BadRequest();
            }

            var pic = Mapper.Map<PicDto, Pic>(picDto);
            _context.Pics.Add(pic);
            _context.SaveChanges();

            picDto.Id = pic.Id;

            return Created(new Uri(Request.RequestUri + "/api/pics/" + picDto.Id), picDto);
        }

        // PUT /api/pic/1
        [HttpPut]
        public IHttpActionResult UpdatePic(int id, PicDto picDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var picInDb = _context.Pics
                .SingleOrDefault(m => m.Id == id);

            if (picInDb == null)
            {
                return NotFound();
            }

            Mapper.Map(picDto, picInDb);

            _context.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // PATCH /api/pic/1
        [HttpPatch]
        public IHttpActionResult LovePic(int id, LovedPicDto lovedDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var picInDb = _context.Pics
                .Include(m => m.Lovers)
                .SingleOrDefault(m => m.Id == id);

            if (picInDb == null)
            {
                return NotFound();
            }

            var userId = User.Identity.GetUserId();

            if (lovedDto.IsLoved)
            {
                picInDb.Lovers.Add(_context.Users.Single(u => u.Id == userId));
            }
            else
            {
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
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            _context.Pics.Remove(pic);
            _context.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
