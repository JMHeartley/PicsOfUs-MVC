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
            var members = _context.Members
                .AsEnumerable()
                .Select(m => new MemberSelectViewModel
                {
                    MemberId = 0,
                    Name = m.Name,
                    IsSelected = false
                }).ToList();

            var viewModel = new PhotoFormViewModel
            {
                Members = members
            };

            return View("PhotoForm", viewModel);
        }
    }
}