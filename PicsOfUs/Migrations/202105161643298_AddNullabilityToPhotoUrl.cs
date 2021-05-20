namespace PicsOfUs.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNullabilityToPhotoUrl : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Photos", "Url", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Photos", "Url", c => c.String(nullable: false));
        }
    }
}
