using FluentMigrator;

namespace IdentityServer.Migrations.Migrations
{
    [Migration(20200404220000)]
    public class AddPermission : Migration
    {
        public override void Up()
        {
            Create.Table("Permissions")
                .WithColumn("id").AsGuid().PrimaryKey()
                .WithColumn("name").AsString(20).NotNullable()
                .WithColumn("display_name").AsString(50).NotNullable()
                .WithColumn("description").AsString(250);

            #region Client
            Create.Table("ClientsPermissions")
                .WithColumn("client_id").AsGuid()
                .WithColumn("permission_id").AsGuid();

            Create.PrimaryKey("PK_ClientsPermissions")
                .OnTable("ClientsPermissions")
                .Columns("client_id", "permission_id");

            Create.ForeignKey("FK_ClientsPermissions_Permissions")
                .FromTable("ClientsPermissions").ForeignColumn("permission_id")
                .ToTable("Permissions").PrimaryColumn("id");
            
            Create.ForeignKey("FK_ClientsPermissions_Clients")
                .FromTable("ClientsPermissions").ForeignColumn("client_id")
                .ToTable("Clients").PrimaryColumn("id");
            
            Create.Index("IX_ClientsPermissions_ClientId")
                .OnTable("ClientsPermissions")
                .OnColumn("client_id");
            #endregion

            #region Users

            Create.Table("UsersPermissions")
                .WithColumn("user_id").AsGuid()
                .WithColumn("permission_id").AsGuid();

            Create.PrimaryKey("PK_UsersPermissions")
                .OnTable("UsersPermissions")
                .Columns("user_id", "permission_id");

            Create.ForeignKey("FK_UsersPermissions_Permissions")
                .FromTable("UsersPermissions").ForeignColumn("permission_id")
                .ToTable("Permissions").PrimaryColumn("id");
            
            Create.ForeignKey("FK_UsersPermissions_Users")
                .FromTable("UsersPermissions").ForeignColumn("user_id")
                .ToTable("Users").PrimaryColumn("id");

            Create.Index("IX_UsersPermissions_ClientId")
                .OnTable("UsersPermissions")
                .OnColumn("user_id");
            #endregion

            #region Roles

            Create.Table("RolesPermissions")
                .WithColumn("role_id").AsGuid()
                .WithColumn("permission_id").AsGuid();
            
            Create.PrimaryKey("PK_RolesPermissions")
                .OnTable("RolesPermissions")
                .Columns("role_id", "permission_id");

            Create.ForeignKey("FK_RolesPermissions_Permissions")
                .FromTable("RolesPermissions").ForeignColumn("permission_id")
                .ToTable("Permissions").PrimaryColumn("id");
            
            Create.ForeignKey("FK_ClientsPermissions_Roles")
                .FromTable("RolesPermissions").ForeignColumn("role_id")
                .ToTable("Roles").PrimaryColumn("id");

            Create.Index("IX_RolesPermissions_RoleId")
                .OnTable("RolesPermissions")
                .OnColumn("role_id");
            
            #endregion
        }

        public override void Down()
        {
            Delete.ForeignKey("FK_ClientsPermissions_Permissions");
            Delete.ForeignKey("FK_ClientsPermissions_Clients");
            Delete.ForeignKey("ClientsPermissions");

            Delete.ForeignKey("FK_UsersPermissions_Permissions");
            Delete.ForeignKey("FK_UsersPermissions_Users");
            Delete.ForeignKey("UsersPermissions");
            
            Delete.ForeignKey("FK_RolesPermissions_Permissions");
            Delete.ForeignKey("FK_ClientsPermissions_Roles");
            Delete.ForeignKey("RolesPermissions");
            
            Delete.Table("Permissions");
        }
    }
}