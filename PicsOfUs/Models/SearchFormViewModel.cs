using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PicsOfUs.Models
{
    public class SearchFormViewModel
    {
        public DateTime? FromCaptureDate { get; set; }
        public DateTime? ToCaptureDate { get; set; }
        public ICollection<MemberSelectViewModel> PicSubjects { get; set; }
    }
}