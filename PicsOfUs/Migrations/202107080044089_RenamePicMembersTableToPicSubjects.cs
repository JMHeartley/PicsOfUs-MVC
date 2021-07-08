namespace PicsOfUs.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenamePicMembersTableToPicSubjects : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.PicMembers", newName: "PicSubjects");
            RenameColumn(table: "dbo.PicSubjects", name: "Pic_Id", newName: "PicId");
            RenameColumn(table: "dbo.PicSubjects", name: "Member_Id", newName: "SubjectId");
            RenameIndex(table: "dbo.PicSubjects", name: "IX_Pic_Id", newName: "IX_PicId");
            RenameIndex(table: "dbo.PicSubjects", name: "IX_Member_Id", newName: "IX_SubjectId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.PicSubjects", name: "IX_SubjectId", newName: "IX_Member_Id");
            RenameIndex(table: "dbo.PicSubjects", name: "IX_PicId", newName: "IX_Pic_Id");
            RenameColumn(table: "dbo.PicSubjects", name: "SubjectId", newName: "Member_Id");
            RenameColumn(table: "dbo.PicSubjects", name: "PicId", newName: "Pic_Id");
            RenameTable(name: "dbo.PicSubjects", newName: "PicMembers");
        }
    }
}
