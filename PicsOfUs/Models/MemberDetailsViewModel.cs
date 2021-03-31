using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PicsOfUs.Models
{
    public class MemberDetailsViewModel
    {
        public Member Member { get; set; }
        public IEnumerable<MiniProfileViewModel> Siblings { get; set; }
        public IEnumerable<MiniProfileViewModel> Parents { get; set; }
        public IEnumerable<MiniProfileViewModel> Children { get; set; }
    }
}