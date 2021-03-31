using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PicsOfUs.Models
{
    public class PicProfileViewModel
    {
        public int MemberId { get; set; }
        public string Name { get; set; }
        public int? AgeInPhoto { get; set; }
    }
}