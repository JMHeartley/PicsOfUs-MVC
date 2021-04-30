﻿using PicsOfUs.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PicsOfUs.Dtos
{
    public class PhotoDto
    {
        public int Id { get; set; }

        [Required]
        public string Url { get; set; }
        public string Caption { get; set; }
        public DateTime? CaptureDate { get; set; }
        public ICollection<MiniMemberDto> Members { get; set; }
        public bool IsLoved { get; set; }
    }
}