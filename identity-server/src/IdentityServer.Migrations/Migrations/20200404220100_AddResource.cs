using FluentMigrator;

namespace IdentityServer.Migrations.Migrations
{
    [Migration(20200404220100)]
    public class AddResource : Migration
    {
        public override void Up()
        {
            Create.Table("Resources")
                .WithColumn("id").AsGuid().PrimaryKey()
                .WithColumn("name").AsString(20).NotNullable().Unique("IX_Resource_Name")
                .WithColumn("display_name").AsString(50).NotNullable()
                .WithColumn("description").AsString(250)
                .WithColumn("is_active").AsBoolean().NotNullable();
            
            #region Client
            Create.Table("ClientsResources")
                .WithColumn("client_id").AsGuid()
                .WithColumn("resource_id").AsGuid();

            Create.PrimaryKey("PK_ClientsResources")
                .OnTable("ClientsResources")
                .Columns("client_id", "resource_id");

            Create.ForeignKey("FK_ClientsResources_Resources")
                .FromTable("ClientsResources").ForeignColumn("resource_id")
                .ToTable("Resources").PrimaryColumn("id");
            
            Create.ForeignKey("FK_ClientsResources_Clients")
                .FromTable("ClientsResources").ForeignColumn("client_id")
                .ToTable("Clients").PrimaryColumn("id");
            #endregion
        }

        public override void Down()
        {
            Delete.ForeignKey("FK_ClientsResources_Resources");
            Delete.ForeignKey("FK_ClientsResources_Clients");
            Delete.Table("ClientsResources");
            
            Delete.Table("Resource");
        }
    }
}