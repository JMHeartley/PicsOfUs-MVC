namespace PicsOfUs.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeedUsers : DbMigration
    {
        public override void Up()
        {
            Sql(@"INSERT INTO [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'0357874e-6deb-4415-a9a9-5498f9415811', N'guest@picsof.us', 0, N'AO8mZ7BumrrfuHmZNju96uUAmSr6BlcpW4XdahAbnRKqije+SXBvH45FWzrOoWaLxQ==', N'05f940c7-0435-49a1-bcab-77f8e1e5654f', NULL, 0, 0, NULL, 1, 0, N'guest@picsof.us')
                  INSERT INTO [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'dad99d6c-94eb-408b-9815-b910ef47c2ed', N'admin@picsof.us', 0, N'AOvN+pZVK6mh/vNUbk/J7iwoWuuJsySSV5CPguObJOzNcUhqsTHqKyaLArT4wbNVgg==', N'5dede48b-b157-4410-96cf-06410c8ea6f1', NULL, 0, 0, NULL, 1, 0, N'admin@picsof.us')

                  INSERT INTO [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'630ca20f-cdf9-4337-862c-132ec23ea078', N'CanManagePicsAndTree'012)

                  INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'dad99d6c-94eb-408b-9815-b910ef47c2ed', N'630ca20f-cdf9-4337-862c-132ec23ea078')"
            );
        }
        
        public override void Down()
        {
        }
    }
}
