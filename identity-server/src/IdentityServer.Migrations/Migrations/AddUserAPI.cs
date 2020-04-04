using System;
using FluentMigrator;

namespace IdentityServer.Migrations.Migrations
{
    public class AddUserAPI : Migration
    {
        public override void Up()
        {
            Insert.IntoTable("Resource").Row(new
            {
                id = Guid.NewGuid(),
                name = "user_api",
                display_name = "User API",
                description = "User Information API",
                enable = true
            });

            Insert.IntoTable("Role")
                .Row(new {id = Guid.NewGuid(), name = "read_user"})
                .Row(new {id = Guid.NewGuid(), name = "write_user"})
                .Row(new {id = Guid.NewGuid(), name = "creat_user"});
        }

        public override void Down()
        {
            Delete.FromTable("Role")
                .Row(new {name = "read_user"})
                .Row(new {name = "write_user"})
                .Row(new {name = "creat_user"});
            
            Delete.FromTable("Resource")
                .Row(new {name = "user_api"});
        }
    }
}