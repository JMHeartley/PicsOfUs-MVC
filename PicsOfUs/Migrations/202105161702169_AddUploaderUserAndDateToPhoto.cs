namespace PicsOfUs.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUploaderUserAndDateToPhoto : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Photos", "UploadDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Photos", "Uploader_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Photos", "Uploader_Id");
            AddForeignKey("dbo.Photos", "Uploader_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Photos", "Uploader_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Photos", new[] { "Uploader_Id" });
            DropColumn("dbo.Photos", "Uploader_Id");
            DropColumn("dbo.Photos", "UploadDate");
        }
    }
}
