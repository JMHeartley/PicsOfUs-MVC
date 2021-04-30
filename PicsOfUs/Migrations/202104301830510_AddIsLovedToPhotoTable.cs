namespace PicsOfUs.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsLovedToPhotoTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Photos", "IsLoved", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Photos", "IsLoved");
        }
    }
}
