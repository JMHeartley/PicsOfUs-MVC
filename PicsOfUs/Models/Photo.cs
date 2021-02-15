using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PicsOfUs.Models
{
    public class Photo
    {
        public int  Id { get; set; }
        public string Url { get; set; }
        public string Caption { get; set; }
        public DateTime? CaptureDate { get; set; }
        public ICollection<Member> Members { get; set; }
    }
}