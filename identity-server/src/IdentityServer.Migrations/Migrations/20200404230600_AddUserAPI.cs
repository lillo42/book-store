using System;
using FluentMigrator;

namespace IdentityServer.Migrations.Migrations
{
    [Migration(20200404230600)]
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

            var read = Guid.NewGuid();
            var write = Guid.NewGuid();
            var create = Guid.NewGuid();
            var delete = Guid.NewGuid();

            Insert.IntoTable("Permissions")
                .Row(new
                {
                    id = read, 
                    name = "read_user", 
                    display_name = "Read Users",
                    description = "Read Users"
                })
                .Row(new
                {
                    id = write, 
                    name = "write_user", 
                    display_name = "Update Users",
                    description = "Update Users"
                })
                .Row(new
                {
                    id = create, 
                    name = "create_user",
                    display_name = "Create Users",
                    description = "Create Users"
                })
                .Row(new
                {
                    id = delete, 
                    name = "delete_user", 
                    display_name = "Delete Users",
                    description = "Delete Users"
                });

            var manger = Guid.NewGuid();
            Insert.IntoTable("Roles")
                .Row(new
                {
                    id = manger, 
                    name = "manager_user",
                    display_name = "Manger users",
                });
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