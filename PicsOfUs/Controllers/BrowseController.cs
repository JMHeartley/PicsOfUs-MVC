using PicsOfUs.Models;
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
            var photos = _context.Photos.ToList();

            return View(photos);
        }
    }
}