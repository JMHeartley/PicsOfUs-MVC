namespace PicsOfUs.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveDimensionsFromPhoto : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Photos", "Height");
            DropColumn("dbo.Photos", "Width");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Photos", "Width", c => c.Int(nullable: false));
            AddColumn("dbo.Photos", "Height", c => c.Int(nullable: false));
        }
    }
}
