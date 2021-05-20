using System;
using System.Collections.Generic;
using PicsOfUs.Models;
using System.Data.Entity;
using System.Diagnostics.Eventing.Reader;
using System.EnterpriseServices;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace PicsOfUs.Controllers
{
    public class BrowseController : Controller
    {
        private ApplicationDbContext _context;

        public BrowseController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        public ActionResult Index(BrowseIndexViewModel viewModel)
        {
            if (viewModel.SearchForm == null)
            {
                viewModel.SearchForm = new SearchFormViewModel
                {
                    PicSubjects = _context.Members
                        .AsEnumerable()
                        .Select(m => new MemberSelectViewModel
                        {
                            MemberId = m.Id,
                            Name = m.Name,
                            IsSelected = false
                        }).ToList()
                };
            }
            else if (ModelState.IsValid)
            {
                var form = viewModel.SearchForm;

                // load photos from database
                var photos = _context.Photos.Include(p => p.Members).AsQueryable();

                // remove photos taken before form.CaptureDateFrom
                if (form.CaptureDateFrom != null)
                {
                    photos = photos.Where(p => p.CaptureDate >= form.CaptureDateFrom);
                }

                // remove photos taken after form.CaptureDateTo
                if (form.CaptureDateTo != null)
                {
                    photos = photos.Where(p => p.CaptureDate <= form.CaptureDateTo);
                }

                // filter photos by form.PicSubjects
                var selectedIds = form.PicSubjects
                    .Where(m => m.IsSelected)
                    .Select(m => m.MemberId)
                    .ToList();

                if (selectedIds.Any())
                {
                    if (form.hasOnlySelectedMembers)
                    {
                        photos = photos.Where(p => p.Members
                                .All(m => selectedIds
                                    .Contains(m.Id)));
                    }
                    else
                    {
                        photos = photos.Where(p => p.Members
                                            .Any(m => selectedIds
                                                .Contains(m.Id)));
                    }
                }

                viewModel.ResultPics = photos.ToList();
            }

            return View(viewModel);
        }

        [SecurityRole(RoleName.CanManagePicsAndTree)]
        public ActionResult New()
        {
            var viewModel = new PhotoFormViewModel
            {
                Photo = new Photo(),
                Members = _context.Members
                    .AsEnumerable()
                    .Select(m => new MemberSelectViewModel
                    {
                        MemberId = m.Id,
                        Name = m.Name,
                        IsSelected = false
                    }).ToList()
            };

            return View("PhotoForm", viewModel);
        }

        [SecurityRole(RoleName.CanManagePicsAndTree)]
        public ActionResult Edit(int id)
        {
            var photo = _context.Photos
                .Include(p => p.Members)
                .SingleOrDefault(p => p.Id == id);

            if (photo == null)
                return HttpNotFound();

            var viewModel = new PhotoFormViewModel
            {
                Photo = photo,
                Members = _context.Members
                    .AsEnumerable()
                    .Select(m => new MemberSelectViewModel
                    {
                        MemberId = m.Id,
                        Name = m.Name,
                        IsSelected = photo.Members.Contains(m)
                    }).ToList()
            };

            return View("PhotoForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SecurityRole(RoleName.CanManagePicsAndTree)]
        public ActionResult Save(PhotoFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                View("PhotoForm", viewModel);
            }

            var photo = viewModel.Photo;

            photo.Url = UploadToFolder(viewModel.File);

            var selectedMemberIds = viewModel.Members
                    .Where(m => m.IsSelected)
                    .Select(m => m.MemberId);

            photo.Members = _context.Members
                .Where(m => selectedMemberIds.Contains(m.Id))
                .ToList();

            if (photo.Id == 0)
            {
                _context.Photos.Add(photo);
            }
            else
            {
                var photoInDb = _context.Photos
                    .Include(p => p.Members)
                    .Single(p => p.Id == viewModel.Photo.Id);

                photoInDb.Url = photo.Url;
                photoInDb.Caption = photo.Caption;
                photoInDb.CaptureDate = photo.CaptureDate;
                photoInDb.Members = photo.Members;
            }

            _context.SaveChanges();

            return RedirectToAction("Index", "Browse");
        }

        private string UploadToFolder(HttpPostedFileBase pic)
        {
            var uploadedFile = new byte[pic.InputStream.Length];
            pic.InputStream.Read(uploadedFile, 0, uploadedFile.Length);

            var fileName = pic.FileName;
            var filePath = Server.MapPath("/Uploads");
            var savedFileName = Path.Combine(filePath, fileName);
            pic.SaveAs(savedFileName);

            var url = "/Uploads/" + fileName;
            return url;
        }
    }
}