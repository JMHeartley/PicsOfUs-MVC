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

                var pics = _context.Pics.Include(p => p.Subjects).AsQueryable();

                if (form.CaptureDateFrom != null)
                {
                    pics = pics.Where(p => p.CaptureDate >= form.CaptureDateFrom);
                }

                if (form.CaptureDateTo != null)
                {
                    pics = pics.Where(p => p.CaptureDate <= form.CaptureDateTo);
                }

                var selectedIds = form.PicSubjects
                    .Where(m => m.IsSelected)
                    .Select(m => m.MemberId)
                    .ToList();

                if (selectedIds.Any())
                {
                    if (form.RequireAll)
                    {
                        pics = pics.Where(p => p.Subjects
                                .All(m => selectedIds
                                    .Contains(m.Id)));
                    }
                    else
                    {
                        pics = pics.Where(p => p.Subjects
                                            .Any(m => selectedIds
                                                .Contains(m.Id)));
                    }
                }

                viewModel.ResultPics = pics.ToList();
            }

            return View(viewModel);
        }

        public ActionResult New()
        {
            var viewModel = new PicFormViewModel
            {
                Pic = new Pic(),
                Members = _context.Members
                    .AsEnumerable()
                    .Select(m => new MemberSelectViewModel
                    {
                        MemberId = m.Id,
                        Name = m.Name,
                        IsSelected = false
                    }).ToList()
            };

            return View("PicForm", viewModel);
        }

        public ActionResult Edit(int id)
        {
            var pic = _context.Pics
                .Include(p => p.Subjects)
                .SingleOrDefault(p => p.Id == id);

            if (pic == null)
                return HttpNotFound();

            var viewModel = new PicFormViewModel
            {
                Pic = pic,
                Members = _context.Members
                    .AsEnumerable()
                    .Select(m => new MemberSelectViewModel
                    {
                        MemberId = m.Id,
                        Name = m.Name,
                        IsSelected = pic.Subjects.Contains(m)
                    }).ToList()
            };

            return View("PicForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(PicFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                View("PicForm", viewModel);
            }

            var pic = viewModel.Pic;

            pic.Url = SaveToUploadsSubfolder(viewModel.File);
            
            var selectedMemberIds = viewModel.Members
                    .Where(m => m.IsSelected)
                    .Select(m => m.MemberId);

            pic.Subjects = _context.Members
                .Where(m => selectedMemberIds.Contains(m.Id))
                .ToList();

            if (pic.Id == 0)
            {
                var userId = User.Identity.GetUserId();
                pic.Uploader = _context.Users.Single(u => u.Id == userId);

                pic.UploadDate = DateTime.Now;

                _context.Pics.Add(pic);
            }
            else
            {
                var picInDb = _context.Pics
                    .Include(p => p.Subjects)
                    .Single(p => p.Id == pic.Id);

                picInDb.Url = pic.Url;
                picInDb.Caption = pic.Caption;
                picInDb.CaptureDate = pic.CaptureDate;
                picInDb.Subjects = pic.Subjects;
            }

            _context.SaveChanges();

            return RedirectToAction("Index", "Browse");
        }

        private string SaveToUploadsSubfolder(HttpPostedFileBase pic)
        {
            //var uploadedFile = new byte[pic.InputStream.Length];
            //pic.InputStream.Read(uploadedFile, 0, uploadedFile.Length);

            var fileName = $"{DateTime.Now:yyyyMMddHHmmssff}{pic.FileName}";

            var userId = User.Identity.GetUserId();
            var appUser = _context.Users.Single(u => u.Id == userId);

            var filePath = Server.MapPath(appUser.UploadsFolder);
            var savedFileName = Path.Combine(filePath, fileName);
            pic.SaveAs(savedFileName);

            var url = $"{appUser.UploadsFolder}/{fileName}"; 
            return url;
        }

        public ActionResult Delete(int id)
        {
            var pic = _context.Pics.SingleOrDefault(p => p.Id == id);

            if (pic == null)
            {
                return HttpNotFound();
            }

            var fullPath = Request.MapPath($"~{pic.Url}");
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }

            _context.Pics.Remove(pic);
            _context.SaveChanges();

            return RedirectToAction("Uploads", "Account"); ;
        }
    }
}