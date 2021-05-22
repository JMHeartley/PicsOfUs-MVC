namespace PicsOfUs.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUploadsFolderToAspNetUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "UploadsFolder", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "UploadsFolder");
        }
    }
}
