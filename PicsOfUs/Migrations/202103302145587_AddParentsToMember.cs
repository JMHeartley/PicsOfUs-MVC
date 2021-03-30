namespace PicsOfUs.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddParentsToMember : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChildParents",
                c => new
                    {
                        ChildId = c.Int(nullable: false),
                        ParentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ChildId, t.ParentId })
                .ForeignKey("dbo.Members", t => t.ChildId)
                .ForeignKey("dbo.Members", t => t.ParentId)
                .Index(t => t.ChildId)
                .Index(t => t.ParentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChildParents", "ParentId", "dbo.Members");
            DropForeignKey("dbo.ChildParents", "ChildId", "dbo.Members");
            DropIndex("dbo.ChildParents", new[] { "ParentId" });
            DropIndex("dbo.ChildParents", new[] { "ChildId" });
            DropTable("dbo.ChildParents");
        }
    }
}
