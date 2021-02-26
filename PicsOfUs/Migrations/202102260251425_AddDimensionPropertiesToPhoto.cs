namespace PicsOfUs.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDimensionPropertiesToPhoto : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Photos", "Height", c => c.Int(nullable: false));
            AddColumn("dbo.Photos", "Width", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Photos", "Width");
            DropColumn("dbo.Photos", "Height");
        }
    }
}
