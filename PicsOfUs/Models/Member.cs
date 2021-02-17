using System;
using System.Collections.Generic;

namespace PicsOfUs.Models
{
    public class Member
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Gender Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public Suffix? Suffix { get; set; }
        public ICollection<Photo> Photos { get; set; }

        public string Name
        {
            get
            {
                if (Suffix == null)
                    return FirstName + " " + LastName;

                return FirstName + " " + LastName + " " + Suffix;
            }
        }
    }

    public enum Gender
    {
        Female = 1,
        Male = 2,
        Other = 3
    }

    public enum Suffix
    {
        I = 1,
        II = 2,
        III = 3,
        IV = 4,
        V = 5
    }
}