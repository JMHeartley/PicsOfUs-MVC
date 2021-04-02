using AutoMapper;
using PicsOfUs.Dtos;
using PicsOfUs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PicsOfUs.Controllers.Api
{
    public class PhotosController : ApiController
    {
        private ApplicationDbContext _context;

        public PhotosController()
        {
            _context = new ApplicationDbContext();
        }

        // GET  /api/photos
        public IHttpActionResult GetPhotos()
        {
            return Ok(_context.Photos.ToList().Select(Mapper.Map<Photo, PhotoDto>));
        }

        // GET /api/photos/1
        public IHttpActionResult GetPhoto(int id)
        {
            var photo = _context.Photos.SingleOrDefault(p => p.Id == id);

            if (photo == null)
            {
                return BadRequest();
            }

            return Ok(Mapper.Map<Photo, PhotoDto>(photo));
        }

        // POST /api/photos
        [HttpPost]
        public IHttpActionResult CreatePhoto(PhotoDto photoDto)
        {
            if (!ModelState.IsValid)
            {
                BadRequest();
            }

            var photo = Mapper.Map<PhotoDto, Photo>(photoDto);
            _context.Photos.Add(photo);
            _context.SaveChanges();

            photoDto.Id = photo.Id;

            return Created(new Uri(Request.RequestUri + "/api/photos/" + photoDto.Id), photoDto);
        }

        // PUT /api/photo/1
        [HttpPut]
        public IHttpActionResult UpdatePhoto(int id, PhotoDto photoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var photoInDb = _context.Photos.SingleOrDefault(m => m.Id == id);

            if (photoInDb == null)
            {
                return NotFound();
            }

            Mapper.Map(photoDto, photoInDb);

            _context.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE /api/photos/1
        [HttpDelete]
        public IHttpActionResult DeletePhoto(int id)
        {
            var photo = _context.Photos.SingleOrDefault(p => p.Id == id);

            if (photo == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            _context.Photos.Remove(photo);
            _context.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
