﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PicsOfUs.Models
{
    public class SearchFormViewModel
    {
        [Display(Name = "From")]
        public DateTime? CaptureDateFrom { get; set; }
        [Display(Name = "To")]
        public DateTime? CaptureDateTo { get; set; }
        public ICollection<MemberSelectViewModel> PicSubjects { get; set; }
        [Display(Name = "Show pics with only the people I selected")]
        public bool hasOnlySelectedMembers { get; set; }
    }
}