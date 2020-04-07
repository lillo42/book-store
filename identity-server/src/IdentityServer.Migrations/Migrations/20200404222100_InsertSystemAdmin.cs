using System;
using System.Security.Cryptography;
using System.Text;
using FluentMigrator;

namespace IdentityServer.Migrations.Migrations
{
    [Migration(20200404222100)]
    public class InsertSystemAdmin : Migration
    {
        public override void Up()
        {
            var userId = Guid.NewGuid();
            Insert.IntoTable("Users")
                .Row(new
                {
                    id = userId,
                    mail = "system@admin.com",
                    password = ComputeHash("123456"),
                    is_active = true
                });

            var roleId = Guid.NewGuid();
            Insert.IntoTable("Role")
                .Row(new
                {
                    id = roleId, 
                    name = "sys_admin", 
                    display_name = "System Admin",
                    description = "System Admin"
                });

            Insert.IntoTable("UsersRoles")
                .Row(new {role_id = roleId, user_id = userId});
        }

        private static string ComputeHash(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return string.Empty;
            }

            using var hashAlgorithm = SHA256.Create();
            var data = Encoding.UTF8.GetBytes(password);
            var hash = hashAlgorithm.ComputeHash(data);
            
            var sb = new StringBuilder();
            foreach (var @byte in hash)
            {
                sb.Append(@byte.ToString("X2"));
            }
            return sb.ToString();
        }
        
        public override void Down()
        {
            Insert.IntoTable("Users")
                .Row(new { mail = "system@admin.com" });
        }
    }
}