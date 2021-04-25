using System.Linq;
using System.Web.Mvc;
using PicsOfUs.Models;
using System.Data.Entity;

namespace PicsOfUs.Controllers
{
    public class MembersController : Controller
    {
        private ApplicationDbContext _context;

        public MembersController()
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
            var viewModel = new MemberFormViewModel
            {
                Member = new Member(),
                Siblings = _context.Members
                    .AsEnumerable()
                    .Select(m => new MemberSelectViewModel
                    {
                        MemberId = m.Id,
                        Name = m.Name,
                        IsSelected = false
                    }).ToList(),
                Parents = _context.Members
                    .AsEnumerable()
                    .Select(m => new MemberSelectViewModel
                    {
                        MemberId = m.Id,
                        Name = m.Name,
                        IsSelected = false
                    }).ToList(),
                Children = _context.Members
                    .AsEnumerable()
                    .Select(m => new MemberSelectViewModel
                    {
                        MemberId = m.Id,
                        Name = m.Name,
                        IsSelected = false
                    }).ToList()
            };

            return View("MemberForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(MemberFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
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
            }

            _context.SaveChanges();

            return RedirectToAction("Index", "Browse");
        }
    }
}