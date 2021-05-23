using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PicsOfUs.Models
{
    public class PicDetailsViewModel
    {
        public Pic Pic { get; set; }
        public IEnumerable<PicProfileViewModel> Subjects { get; set; }
    }
}