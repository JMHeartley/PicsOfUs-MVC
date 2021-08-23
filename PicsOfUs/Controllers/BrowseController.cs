using Microsoft.AspNet.Identity;
using PicsOfUs.Models;
using PicsOfUs.Utilities;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

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
        public ActionResult Index()
        {
            var viewModel = new BrowseIndexViewModel
            {
                SearchForm = new SearchFormViewModel
                {
                    PicSubjects = _context.Members
                    .AsEnumerable()
                    .Select(m => new MemberSelectViewModel
                    {
                        MemberId = m.Id,
                        Name = m.Name,
                        IsSelected = false
                    }).ToList()
                }
            };

            NLogger.GetInstance().Info($"User (id: {User.Identity.GetUserId()}) requested Browse index");
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Index(BrowseIndexViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                NLogger.GetInstance().Warning($"Browse index form not valid for user (id: {User.Identity.GetUserId()})");
                return View(viewModel);
            }

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

            if (form.PicSubjects != null)
            {
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
            }

            var groups = pics.GroupBy(p => p.CaptureDate ?? default);

            switch (form.SortBy)
            {
                case SortBy.NewestFirst:
                    viewModel.ResultGroups = groups.OrderByDescending(g => g.Key).ToList();
                    break;
                case SortBy.OldestFirst:
                    viewModel.ResultGroups = groups.OrderBy(g => g.Key).ToList();
                    break;
                default:
                    NLogger.GetInstance().Error("SortBy had an ArgumentOutOfRange Exception", form.SortBy.ToString());
                    throw new ArgumentOutOfRangeException();
            }

            NLogger.GetInstance().Info($"User (id: {User.Identity.GetUserId()} filtered pics with Browse index form");
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

            NLogger.GetInstance().Info($"User (id: {User.Identity.GetUserId()}) requested new pic form");
            return View("PicForm", viewModel);
        }

        public ActionResult Edit(int id)
        {
            var pic = _context.Pics
                .Include(p => p.Subjects)
                .SingleOrDefault(p => p.Id == id);

            if (pic == null)
            {
                NLogger.GetInstance().Warning($"User (id: {User.Identity.GetUserId()}) requested edit pic form, pic (id: {id}) not found");
                return HttpNotFound();
            }

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

            NLogger.GetInstance().Info($"User (id: {User.Identity.GetUserId()}) requested edit pic form (pic id: {pic.Id})");
            return View("PicForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(PicFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                NLogger.GetInstance().Warning($"Pic form not valid for user (id: {User.Identity.GetUserId()})");
                return View("PicForm", viewModel);
            }

            if (!IsSupportedImageType(viewModel.File))
            {
                NLogger.GetInstance().Warning($"User (id: {User.Identity.GetUserId()}) tried saving an unsupported file type");
                TempData["Error"] = "The uploaded image is an unsupported file type.";
                return View("PicForm", viewModel);
            }

            var pic = viewModel.Pic;

            pic.Url = SaveToUploadsSubfolder(viewModel.File);

            if (viewModel.Members != null)
            {
                var selectedMemberIds = viewModel.Members
                        .Where(m => m.IsSelected)
                        .Select(m => m.MemberId);

                pic.Subjects = _context.Members
                    .Where(m => selectedMemberIds.Contains(m.Id))
                    .ToList();
            }

            if (pic.Id == 0)
            {
                var userId = User.Identity.GetUserId();
                pic.Uploader = _context.Users.Single(u => u.Id == userId);

                pic.UploadDate = DateTime.Now;

                _context.Pics.Add(pic);
                NLogger.GetInstance().Info($"User (id: {User.Identity.GetUserId()}) created member (id: {pic.Id})");
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

                NLogger.GetInstance().Info($"User (id: {User.Identity.GetUserId()}) edited member (id: {pic.Id})");
            }

            _context.SaveChanges();

            return RedirectToAction("Index", "Browse");
        }

        private string SaveToUploadsSubfolder(HttpPostedFileBase pic)
        {
            var fileName = $"{DateTime.Now:MM-dd-yyyy--HH-mm-ss-ff}{Path.GetExtension(pic.FileName)}";

            var userId = User.Identity.GetUserId();
            var appUser = _context.Users.Single(u => u.Id == userId);

            var filePath = Server.MapPath(appUser.UploadsFolder);
            var savedFileName = Path.Combine(filePath, fileName);
            pic.SaveAs(savedFileName);

            var url = $"{appUser.UploadsFolder}/{fileName}";
            return url;
        }

        private bool IsSupportedImageType(HttpPostedFileBase file)
        {
            if (file.ContentType.Contains("image"))
            {
                return true;
            }

            var supportedFormats = new[] { ".jpg", ".png", ".heic", ".ciff", ".jpeg" };
            return supportedFormats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }

        public ActionResult Delete(int id)
        {
            var pic = _context.Pics.SingleOrDefault(p => p.Id == id);

            if (pic == null)
            {
                NLogger.GetInstance().Warning($"User (id: {User.Identity.GetUserId()}) requested to delete pic, pic (id: {id}) not found");
                return HttpNotFound();
            }

            var fullPath = Request.MapPath($"~{pic.Url}");
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }

            _context.Pics.Remove(pic);
            _context.SaveChanges();

            NLogger.GetInstance().Info($"User (id: {User.Identity.GetUserId()}) deleted pic (id: {pic.Id})");
            return RedirectToAction("Uploads", "Account"); ;
        }
    }
}