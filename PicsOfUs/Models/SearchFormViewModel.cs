using System;
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

        [Display(Name = "Show pics with only the selected people")]
        public bool RequireAll { get; set; }

        [Display(Name = "Order")]
        public SortBy SortBy { get; set; }
    }

    public enum SortBy
    {
        [Display(Name = "Newest First")]
        NewestFirst,

        [Display(Name = "Oldest First")]
        OldestFirst
    }
}