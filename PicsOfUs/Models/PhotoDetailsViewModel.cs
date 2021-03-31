using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PicsOfUs.Models
{
    public class PhotoDetailsViewModel
    {
        public Photo Photo { get; set; }
        public IEnumerable<PicProfileViewModel> Subjects { get; set; }
    }
}