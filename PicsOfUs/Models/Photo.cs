﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PicsOfUs.Models
{
    public class Photo
    {
        public int Id { get; set; }

        [Required]
        public string Url { get; set; }
        public string Caption { get; set; }
        public DateTime? CaptureDate { get; set; }
        public ICollection<Member> Members { get; set; }
        public ICollection<ApplicationUser> Users { get; set; }
        public bool IsLoved { get; set; }
    }
}