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
                Members = _context.Members.ToList();
            }

            return View("PhotoFormView");
        }
    }
}