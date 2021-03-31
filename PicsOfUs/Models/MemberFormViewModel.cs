using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PicsOfUs.Models
{
    public class MemberFormViewModel
    {
        public Member Member { get; set; }
        
        public IEnumerable<MemberSelectViewModel> Siblings { get; set; }
        
        public IEnumerable<MemberSelectViewModel> Parents { get; set; }
        
        public IEnumerable<MemberSelectViewModel> Children { get; set; }
    }
}