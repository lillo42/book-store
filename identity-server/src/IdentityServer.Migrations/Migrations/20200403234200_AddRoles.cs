using FluentMigrator;

namespace IdentityServer.Migrations.Migrations
{
    [Migration(20200403234200)]
    public class AddRoles : Migration
    {
        public override void Up()
        {
            Create.Table("Roles")
                .WithColumn("id").AsGuid().PrimaryKey("PK_Roles")
                .WithColumn("name").AsString(20).NotNullable().Unique("IX_Roles_Name")
                .WithColumn("display_name").AsString().NotNullable()
                .WithColumn("description").AsString(250);

            Create.Index("IX_Roles_Id")
                .OnTable("Roles")
                .OnColumn("id").Unique();

            #region Client

            Create.Table("ClientsRoles")
                .WithColumn("client_id").AsGuid()
                .WithColumn("role_id").AsGuid();

            Create.PrimaryKey("PK_ClientsRoles")
                .OnTable("ClientsRoles")
                .Columns("client_id", "role_id");

            Create.ForeignKey("FK_ClientsRoles_Roles")
                .FromTable("ClientsRoles").ForeignColumn("role_id")
                .ToTable("Roles").PrimaryColumn("id");
            
            Create.ForeignKey("FK_ClientsRoles_Clients")
                .FromTable("ClientsRoles").ForeignColumn("client_id")
                .ToTable("Clients").PrimaryColumn("id");

            Create.Index("IX_ClientsRoles_ClientId")
                .OnTable("ClientsRoles")
                .OnColumn("client_id");
            #endregion

            #region Users
            Create.Table("UsersRoles")
                .WithColumn("user_id").AsGuid()
                .WithColumn("role_id").AsGuid();

            Create.PrimaryKey("PK_UsersRoles")
                .OnTable("UsersRoles")
                .Columns("user_id", "role_id");

            Create.ForeignKey("FK_UsersRoles_Roles")
                .FromTable("UsersRoles").ForeignColumn("role_id")
                .ToTable("Roles").PrimaryColumn("id");
            
            Create.ForeignKey("FK_UsersRoles_Users")
                .FromTable("UsersRoles").ForeignColumn("user_id")
                .ToTable("Users").PrimaryColumn("id");
            
            Create.Index("IX_UsersRoles_UserId")
                .OnTable("UsersRoles")
                .OnColumn("user_id");

            #endregion
        }

        public override void Down()
        {
            Delete.ForeignKey("FK_ClientsRoles_Clients");
            Delete.ForeignKey("FK_ClientsRoles_Roles");
            Delete.Table("ClientsRoles");

            Delete.ForeignKey("FK_UsersRoles_Roles");
            Delete.ForeignKey("FK_UsersRoles_Users");
            Delete.Table("UsersRoles");
            
            Delete.Table("Roles");
        }
    }
}