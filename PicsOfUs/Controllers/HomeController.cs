using System.Web.Mvc;

namespace PicsOfUs.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}