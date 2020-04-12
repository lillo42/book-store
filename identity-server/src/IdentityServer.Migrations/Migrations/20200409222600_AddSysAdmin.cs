using System;
using FluentMigrator;
using FluentMigrator.Builders.Delete;
using FluentMigrator.Builders.Insert;

namespace IdentityServer.Migrations.Migrations
{
    [Migration(20200409222600)]
    public class AddSysAdmin : Migration
    {
        private static readonly Guid SysAdminRole = Guid.Parse("6ae045f4-3cac-4180-97aa-8aede012c0ba");
        
        public override void Up()
        {
            Insert.Role(SysAdminRole, "sys_admin", "Sys Admin", "System Admin", new Guid[0]);
        }

        public override void Down()
        {
            Delete.Role(SysAdminRole);
        }
    }
}