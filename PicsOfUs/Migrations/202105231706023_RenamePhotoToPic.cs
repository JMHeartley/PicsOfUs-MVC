namespace PicsOfUs.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenamePhotoToPic : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Photos", newName: "Pics");
            RenameTable(name: "dbo.PhotoMembers", newName: "PicSubjects");
            RenameColumn(table: "dbo.PicSubjects", name: "Photo_Id", newName: "Pic_Id");
            RenameIndex(table: "dbo.PicSubjects", name: "IX_Photo_Id", newName: "IX_Pic_Id");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.PicSubjects", name: "IX_Pic_Id", newName: "IX_Photo_Id");
            RenameColumn(table: "dbo.PicSubjects", name: "Pic_Id", newName: "Photo_Id");
            RenameTable(name: "dbo.PicSubjects", newName: "PhotoMembers");
            RenameTable(name: "dbo.Pics", newName: "Photos");
        }
    }
}
