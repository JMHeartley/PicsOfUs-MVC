using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PicsOfUs.Models
{
    public class Pic
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Caption { get; set; }
        public DateTime? CaptureDate { get; set; }
        public ICollection<Member> Subjects { get; set; }
        public ICollection<ApplicationUser> Lovers { get; set; }
        public bool IsLoved { get; set; }
        public ApplicationUser Uploader { get; set; }
        public DateTime UploadDate { get; set; }
    }
}