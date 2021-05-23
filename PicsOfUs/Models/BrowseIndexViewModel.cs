using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PicsOfUs.Models
{
    public class BrowseIndexViewModel
    {
        public SearchFormViewModel SearchForm { get; set; }
        public List<Pic> ResultPics { get; set; }
    }
}