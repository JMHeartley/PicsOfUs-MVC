using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PicsOfUs.Dtos;
using PicsOfUs.Models;

namespace PicsOfUs.Dtos
{
    public class MiniMemberDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Gender Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public Suffix? Suffix { get; set; }
    }
}