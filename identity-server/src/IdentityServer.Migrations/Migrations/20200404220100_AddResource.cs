using FluentMigrator;

namespace IdentityServer.Migrations.Migrations
{
    [Migration(20200404220100)]
    public class AddResource : Migration
    {
        public override void Up()
        {
            Create.Table("Resource")
                .WithColumn("id").AsGuid().PrimaryKey()
                .WithColumn("name").AsString(20).NotNullable().Unique("IX_Resource_Name")
                .WithColumn("display_name").AsString(50)
                .WithColumn("description").AsString(250)
                .WithColumn("enable").AsBoolean().NotNullable();
        }

        public override void Down()
        {
            Delete.Table("Resource");
        }
    }
}