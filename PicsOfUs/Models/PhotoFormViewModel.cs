using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace PicsOfUs.Models
{
    public class PhotoFormViewModel
    {
        [Required]
        public HttpPostedFileBase File { get; set; }
        public Photo Photo { get; set; }
        public IEnumerable<MemberSelectViewModel> Members { get; set; }
    }
}