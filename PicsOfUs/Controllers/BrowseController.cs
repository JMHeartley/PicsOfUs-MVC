using PicsOfUs.Models;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

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
            var photos = _context.Photos
                .Include(p => p.Members)
                .ToList();

            return View(photos);
        }

        public ActionResult New()
        {
            var viewModel = new PhotoFormViewModel
            {
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

        public ActionResult Save(PhotoFormViewModel viewModel)
        {
            var photo = viewModel.Photo;

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

        public ActionResult Details(int id)
        {
            var photo = _context.Photos
                .Include(p => p.Members)
                .SingleOrDefault(p => p.Id == id);

            return View(photo);
        }
    }
}