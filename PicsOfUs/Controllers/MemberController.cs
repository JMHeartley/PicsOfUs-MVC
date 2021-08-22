using Microsoft.AspNet.Identity;
using PicsOfUs.Models;
using PicsOfUs.Utilities;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace PicsOfUs.Controllers
{
    public class MemberController : Controller
    {
        private ApplicationDbContext _context;

        public MemberController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        public ActionResult Index()
        {
            return RedirectToAction("Index", "Browse");
        }

        public ActionResult New()
        {
            var memberSelectViewModels = _context.Members
                .AsEnumerable()
                .Select(m => new MemberSelectViewModel
                {
                    MemberId = m.Id,
                    Name = m.Name,
                    IsSelected = false
                }).ToList();

            var viewModel = new MemberFormViewModel
            {
                Member = new Member(),
                Siblings = memberSelectViewModels,
                Parents = memberSelectViewModels,
                Children = memberSelectViewModels
            };

            NLogger.GetInstance().Info($"User (id: {User.Identity.GetUserId()}) requested new member form");
            return View("MemberForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(MemberFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                NLogger.GetInstance().Warning($"Member form not valid for user (id: {User.Identity.GetUserId()})");
                return View("MemberForm", viewModel);
            }

            var member = viewModel.Member;

            var selectedSiblingIds = viewModel.Siblings
                .Where(m => m.IsSelected)
                .Select(m => m.MemberId);

            member.Siblings = _context.Members
                .Where(m => selectedSiblingIds.Contains(m.Id))
                .ToList();

            var selectedParentIds = viewModel.Parents
                .Where(m => m.IsSelected)
                .Select(m => m.MemberId);

            member.Parents = _context.Members
                .Where(m => selectedParentIds.Contains(m.Id))
                .ToList();

            var selectedChildrenIds = viewModel.Children
                .Where(m => m.IsSelected)
                .Select(m => m.MemberId);

            member.Children = _context.Members
                .Where(m => selectedChildrenIds.Contains(m.Id))
                .ToList();

            if (member.Id == 0)
            {
                _context.Members.Add(member);
                NLogger.GetInstance().Info($"User (id: {User.Identity.GetUserId()}) created member (id: {member.Id})");
            }
            else
            {
                var memberInDb = _context.Members
                    .Include(m => m.Siblings)
                    .Include(m => m.Parents)
                    .Include(m => m.Children)
                    .Single(m => m.Id == member.Id);

                memberInDb.FirstName = member.FirstName;
                memberInDb.LastName = member.LastName;
                memberInDb.LastName = member.LastName;
                memberInDb.Gender = member.Gender;
                memberInDb.Suffix = member.Suffix;
                memberInDb.Siblings = member.Siblings;
                memberInDb.Parents = member.Parents;
                memberInDb.Children = member.Children;

                NLogger.GetInstance().Info($"User (id: {User.Identity.GetUserId()}) edited member (id: {member.Id})");
            }

            _context.SaveChanges();

            return RedirectToAction("Index", "Browse");
        }
    }
}