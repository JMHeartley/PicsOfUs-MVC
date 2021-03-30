namespace PicsOfUs.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSiblingsToMember : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MemberSiblings",
                c => new
                    {
                        MemberId = c.Int(nullable: false),
                        SiblingId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.MemberId, t.SiblingId })
                .ForeignKey("dbo.Members", t => t.MemberId)
                .ForeignKey("dbo.Members", t => t.SiblingId)
                .Index(t => t.MemberId)
                .Index(t => t.SiblingId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MemberSiblings", "SiblingId", "dbo.Members");
            DropForeignKey("dbo.MemberSiblings", "MemberId", "dbo.Members");
            DropIndex("dbo.MemberSiblings", new[] { "SiblingId" });
            DropIndex("dbo.MemberSiblings", new[] { "MemberId" });
            DropTable("dbo.MemberSiblings");
        }
    }
}
