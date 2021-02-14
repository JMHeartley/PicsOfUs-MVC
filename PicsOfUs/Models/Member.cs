namespace PicsOfUs.Models
{
    public class Member
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Gender Gender { get; set; }
        public Suffix? Suffix { get; set; }
    }

    public enum Gender
    {
        Male = 1,
        Female = 2,
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