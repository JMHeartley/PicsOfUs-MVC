namespace PicsOfUs.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserLovedPicsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LovedPics",
                c => new
                    {
                        PicId = c.String(nullable: false, maxLength: 128),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.PicId, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.PicId, cascadeDelete: true)
                .ForeignKey("dbo.Photos", t => t.UserId, cascadeDelete: true)
                .Index(t => t.PicId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LovedPics", "UserId", "dbo.Photos");
            DropForeignKey("dbo.LovedPics", "PicId", "dbo.AspNetUsers");
            DropIndex("dbo.LovedPics", new[] { "UserId" });
            DropIndex("dbo.LovedPics", new[] { "PicId" });
            DropTable("dbo.LovedPics");
        }
    }
}
