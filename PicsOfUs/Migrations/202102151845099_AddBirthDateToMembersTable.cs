namespace PicsOfUs.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBirthDateToMembersTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Members", "BirthDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Members", "BirthDate");
        }
    }
}
