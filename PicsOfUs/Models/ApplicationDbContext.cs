using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace PicsOfUs.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Member>().HasMany(m => m.Siblings).WithMany().Map(m =>
            {
                m.MapLeftKey("MemberId");
                m.MapRightKey("SiblingId");
                m.ToTable("MemberSiblings");
            });

            modelBuilder.Entity<Member>().HasMany(m => m.Parents).WithMany(m => m.Children).Map(m =>
            {
                m.MapLeftKey("ChildId");
                m.MapRightKey("ParentId");
                m.ToTable("ChildParents");
            });

            modelBuilder.Entity<Pic>().HasMany(m => m.Lovers).WithMany(m => m.LovedPics).Map(m =>
            {
                m.MapLeftKey("PicId");
                m.MapRightKey("UserId");
                m.ToTable("LovedPics");
            });

            modelBuilder.Entity<Pic>().HasMany(m => m.Subjects).WithMany(m => m.Pics).Map(m =>
            {
                m.MapLeftKey("PicId");
                m.MapRightKey("SubjectId");
                m.ToTable("PicSubjects");
            });
        }

        public DbSet<Member> Members { get; set; }
        public DbSet<Pic> Pics { get; set; }
    }
}