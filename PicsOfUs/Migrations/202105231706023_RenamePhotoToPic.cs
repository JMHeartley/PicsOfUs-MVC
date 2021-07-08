namespace PicsOfUs.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenamePhotoToPic : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Photos", newName: "Pics");
            RenameTable(name: "dbo.PhotoMembers", newName: "PicMembers");
            RenameColumn(table: "dbo.PicMembers", name: "Photo_Id", newName: "Pic_Id");
            RenameIndex(table: "dbo.PicMembers", name: "IX_Photo_Id", newName: "IX_Pic_Id");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.PicMembers", name: "IX_Pic_Id", newName: "IX_Photo_Id");
            RenameColumn(table: "dbo.PicMembers", name: "Pic_Id", newName: "Photo_Id");
            RenameTable(name: "dbo.PicMembers", newName: "PhotoMembers");
            RenameTable(name: "dbo.Pics", newName: "Photos");
        }
    }
}
