using System;
using FluentMigrator;
using FluentMigrator.Builders.Delete;
using FluentMigrator.Builders.Insert;

namespace IdentityServer.Migrations.Migrations
{
    [Migration(20200410222600)]
    public class AddUserAPI : Migration
    {
     
        public static readonly Guid UserResource =  Guid.Parse("db08e398-89e7-4488-869c-7ef29dbff340");
        
        public static readonly Guid ReadPermissions = Guid.Parse("3e6729a7-b9d0-4111-91d9-e40f2f663a74");
        public static readonly Guid WritePermissions = Guid.Parse("cfd53b49-4efd-45ce-9dce-16a7c49df687");
        public static readonly Guid DeletePermissions = Guid.Parse("317633cc-6527-4a37-88f1-cb42c787cdec");
        public static readonly Guid CreatePermissions = Guid.Parse("4b84d6ab-fcb0-45a3-9b6a-ad34b709e0f0");
        
        public static readonly Guid MangerRole = Guid.Parse("59171eaa-d695-48a3-890e-8e71949056f1");
        public static readonly Guid ReadOnlyRole = Guid.Parse("cb3d8d32-d794-4738-9e67-ff997b651732");
        public static readonly Guid WriteRole = Guid.Parse("efb1bc29-0e3e-41cd-8f4a-fcd244c47897");
        
        public override void Up()
        {
            Insert.Resource(UserResource, "users", "User API", "User API", true);
            
            Insert.Permission( ReadPermissions, "read_user", "Read User", "Read User");
            Insert.Permission( WritePermissions, "write_user", "Update User", "Update User");
            Insert.Permission( CreatePermissions, "create_user", "Create User", "Create User");
            Insert.Permission( DeletePermissions, "delete_user", "Delete User", "Delete User");

            Insert.Role(MangerRole, "manager_user", "Manger User", "Manager user", new []
            {
                ReadPermissions, WritePermissions, CreatePermissions, DeletePermissions
            });
            
            Insert.Role(ReadOnlyRole, "read_user", "Read Only User", "Read Only user", new []
            {
                ReadPermissions
            });
            
            Insert.Role(WriteRole, "write_user", "Write User", "Write user", new []
            {
                ReadPermissions, WritePermissions
            });
        }

        public override void Down()
        {
            Delete.Role(MangerRole);
            Delete.Role(ReadOnlyRole);
            Delete.Role(WriteRole);
            
            Delete.Permission(ReadPermissions);
            Delete.Permission(WritePermissions);
            Delete.Permission(DeletePermissions);
            Delete.Permission(CreatePermissions);
            
            Delete.Resource(UserResource);
        }
    }
}