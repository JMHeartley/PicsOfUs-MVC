namespace PicsOfUs.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPhotosTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Photos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Url = c.String(),
                        Caption = c.String(),
                        CaptureDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PhotoMembers",
                c => new
                    {
                        Photo_Id = c.Int(nullable: false),
                        Member_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Photo_Id, t.Member_Id })
                .ForeignKey("dbo.Photos", t => t.Photo_Id, cascadeDelete: true)
                .ForeignKey("dbo.Members", t => t.Member_Id, cascadeDelete: true)
                .Index(t => t.Photo_Id)
                .Index(t => t.Member_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PhotoMembers", "Member_Id", "dbo.Members");
            DropForeignKey("dbo.PhotoMembers", "Photo_Id", "dbo.Photos");
            DropIndex("dbo.PhotoMembers", new[] { "Member_Id" });
            DropIndex("dbo.PhotoMembers", new[] { "Photo_Id" });
            DropTable("dbo.PhotoMembers");
            DropTable("dbo.Photos");
        }
    }
}
