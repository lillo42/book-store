using System;
using FluentMigrator;

namespace IdentityServer.Migrations.Migrations
{
    [Migration(20200404222100)]
    public class InsertSystemAdmin : Migration
    {
        public override void Up()
        {
            Insert.IntoTable("Users")
                .Row(new
                {
                    id = Guid.NewGuid(),
                    mail = "system@admin.com",
                    password = "123456",
                    is_active = true
                });
        }

        public override void Down()
        {
            Insert.IntoTable("Users")
                .Row(new { mail = "system@admin.com" });
        }
    }
}