using System.Collections.Generic;
using System.Web;

namespace PicsOfUs.Models
{
    public class PicFormViewModel
    {
        [RequiredIfPicDoesNotExist("Pic")]
        public HttpPostedFileBase File { get; set; }
        public Pic Pic { get; set; }
        public IEnumerable<MemberSelectViewModel> Members { get; set; }
    }
}