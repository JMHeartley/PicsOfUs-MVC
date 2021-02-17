namespace PicsOfUs.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddValidationToPhoto : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Photos", "Url", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Photos", "Url", c => c.String());
        }
    }
}
