using System.Collections.Generic;

namespace PicsOfUs.Models
{
    public class PhotoFormViewModel
    {
        public Photo Photo { get; set; }
        public IEnumerable<MemberSelectViewModel> Members { get; set; }
    }
}