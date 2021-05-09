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

            modelBuilder.Entity<ApplicationUser>().HasMany(m => m.LovedPics).WithMany(m => m.Lovers).Map(m =>
            {
                m.MapLeftKey("PicId");
                m.MapRightKey("UserId");
                m.ToTable("LovedPics");
            });
        }

        public DbSet<Member> Members { get; set; }
        public DbSet<Photo> Photos { get; set; }
    }
}