using System;
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
        public int Height { get; set; }
        public int Width { get; set; }

        public string CaptureDateDisplay
        {
            get
            {
                if (CaptureDate == null)
                    return "";

                return (CaptureDate.GetValueOrDefault()).ToString("{0:MM/dd/yyyy}");
            }
        }
    }
}