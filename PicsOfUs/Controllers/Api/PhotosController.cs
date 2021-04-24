﻿using AutoMapper;
using PicsOfUs.Dtos;
using PicsOfUs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;

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
            return Ok(
                _context.Photos
                    .Include(p => p.Members)
                    .ToList()
                    .Select(Mapper.Map<Photo, PhotoDto>)
                );
        }

        // GET /api/photos/1
        public IHttpActionResult GetPhoto(int id)
        {
            var photo = _context.Photos
                .Include(p => p.Members)
                .SingleOrDefault(p => p.Id == id);

            if (photo == null)
            {
                return BadRequest();
            }

            return Ok(Mapper.Map<Photo, PhotoDto>(photo));
        }

        // POST /api/photos
        [Authorize(Roles = RoleName.CanManagePicsAndTree)]
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
        [Authorize(Roles = RoleName.CanManagePicsAndTree)]
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
        [Authorize(Roles = RoleName.CanManagePicsAndTree)]
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
