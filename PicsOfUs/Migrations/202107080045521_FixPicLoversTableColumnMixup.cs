namespace PicsOfUs.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixPicLoversTableColumnMixup : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.LovedPics", new[] { "PicId" });
            DropIndex("dbo.LovedPics", new[] { "UserId" });
            RenameColumn(table: "dbo.LovedPics", name: "PicId", newName: "__mig_tmp__0");
            RenameColumn(table: "dbo.LovedPics", name: "UserId", newName: "PicId");
            RenameColumn(table: "dbo.LovedPics", name: "__mig_tmp__0", newName: "UserId");
            DropPrimaryKey("dbo.LovedPics");
            AlterColumn("dbo.LovedPics", "PicId", c => c.Int(nullable: false));
            AlterColumn("dbo.LovedPics", "UserId", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.LovedPics", new[] { "PicId", "UserId" });
            CreateIndex("dbo.LovedPics", "PicId");
            CreateIndex("dbo.LovedPics", "UserId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.LovedPics", new[] { "UserId" });
            DropIndex("dbo.LovedPics", new[] { "PicId" });
            DropPrimaryKey("dbo.LovedPics");
            AlterColumn("dbo.LovedPics", "UserId", c => c.Int(nullable: false));
            AlterColumn("dbo.LovedPics", "PicId", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.LovedPics", new[] { "PicId", "UserId" });
            RenameColumn(table: "dbo.LovedPics", name: "UserId", newName: "__mig_tmp__0");
            RenameColumn(table: "dbo.LovedPics", name: "PicId", newName: "UserId");
            RenameColumn(table: "dbo.LovedPics", name: "__mig_tmp__0", newName: "PicId");
            CreateIndex("dbo.LovedPics", "UserId");
            CreateIndex("dbo.LovedPics", "PicId");
        }
    }
}
