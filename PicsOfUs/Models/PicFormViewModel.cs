using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace PicsOfUs.Models
{
    public class PicFormViewModel
    {
        [Required]
        public HttpPostedFileBase File { get; set; }
        public Pic Pic { get; set; }
        public IEnumerable<MemberSelectViewModel> Members { get; set; }
    }
}