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
        public ICollection<Member> Siblings { get; set; }
        public ICollection<Member> Parents { get; set; }
        public ICollection<Member> Children { get; set; }
        public string Name
        {
            get
            {
                if (Suffix == null)
                    return FirstName + " " + LastName;

                return FirstName + " " + LastName + " " + Suffix;
            }
        }

        public string Birthday
        {
            get
            {
                if (BirthDate == null)
                    return "";

                var birthdate = BirthDate.GetValueOrDefault();

                return $"{(Month)birthdate.Month} {birthdate.Day}";
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

    public enum Month
    {
        NotSet = 0,
        January = 1,
        February = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12
    }
}