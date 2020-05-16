using System;
using FluentMigrator;
using FluentMigrator.Builders.Delete;
using FluentMigrator.Builders.Insert;
using IdentityServer.Infrastructure.Abstractions;

namespace IdentityServer.Migrations.Migrations
{
    [Migration(20200409222600)]
    public class AddSysAdmin : Migration
    {
        private readonly IHashAlgorithm _hash;
        private static readonly Guid SysAdminRole = Guid.Parse("6ae045f4-3cac-4180-97aa-8aede012c0ba");
        private static readonly Guid UserSysAdmin = Guid.Parse("f2e68b0f-e895-4444-b000-81610bcbaaee");

        public AddSysAdmin(IHashAlgorithm hash)
        {
            _hash = hash;
        }

        public override void Up()
        {
            Insert.Role(SysAdminRole, "sys_admin", "Sys Admin", "System Admin", new Guid[0]);
            Insert.User(UserSysAdmin, "sys@admin.com", _hash.ComputeHash("123456"), true, new []{SysAdminRole});
        }

        public override void Down()
        {
            Delete.User(UserSysAdmin);
            Delete.Role(SysAdminRole);
        }
    }
}