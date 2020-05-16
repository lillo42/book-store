using System;
using FluentMigrator;
using FluentMigrator.Builders.Delete;
using FluentMigrator.Builders.Insert;

namespace IdentityServer.Migrations.Migrations
{
    [Migration(20200420141700)]
    public class AddAdminAndFrontClient : Migration
    {
        public static readonly Guid Admin = Guid.Parse("ddf15748-c038-4970-b3ed-8557311bf40b");
        public static readonly Guid FrontEnd = Guid.Parse("04203b3e-ea01-4953-a685-7e2111432ffc");
        
        private const string AdminClientId = "1a56f800-2fc9-4b41-b792-5a47f7f39f57";
        private const string AdminClientSecret = "8ea0ea42-edd8-40e3-810e-5cc1978ea140";
        
        private const string FrontEndClientId = "d25de659-a65c-4fe2-a4e0-8e4de0d896db";
        private const string FrontEndClientSecret = "d25de659-a65c-4fe2-a4e0-8e4de0d896db";

        public override void Up()
        {
            Insert.Client(Admin, "Admin Orchestrator", AdminClientId, AdminClientSecret, true,
                new []{ AddUserAPI.UserResource },
                new []{AddUserAPI.MangerRole, AddUserAPI.ReadOnlyRole, AddUserAPI.WriteRole}, 
                new []{AddUserAPI.ReadPermissions, AddUserAPI.WritePermissions, AddUserAPI.DeletePermissions, AddUserAPI.CreatePermissions});
            
            Insert.Client(FrontEnd, "FrontEnd Orchestrator", FrontEndClientId, FrontEndClientSecret, true,
                new []{ AddUserAPI.UserResource },
                new []{AddUserAPI.MangerRole, AddUserAPI.ReadOnlyRole, AddUserAPI.WriteRole}, 
                new []{AddUserAPI.ReadPermissions, AddUserAPI.WritePermissions, AddUserAPI.DeletePermissions, AddUserAPI.CreatePermissions});
        }

        public override void Down()
        {
            Delete.Client(Admin);
            Delete.Client(FrontEnd);
        }
    }
}