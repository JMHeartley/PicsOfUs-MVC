using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PicsOfUs.Models;

namespace PicsOfUs.Dtos
{
    public class MemberDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Gender Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public Suffix? Suffix { get; set; }
        public ICollection<Member> Siblings { get; set; }
        public ICollection<Member> Parents { get; set; }
        public ICollection<Member> Children { get; set; }
    }
}