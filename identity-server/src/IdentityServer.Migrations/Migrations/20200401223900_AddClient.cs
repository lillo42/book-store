using FluentMigrator;

namespace IdentityServer.Migrations.Migrations
{
    [Migration(20200401223900)]
    public class AddClient : Migration
    {
        public override void Up()
        {
            Create.Table("Clients")
                .WithColumn("id").AsGuid().NotNullable().PrimaryKey("PK_Clients_Id")
                .WithColumn("name").AsString(100).NotNullable()
                .WithColumn("is_active").AsBoolean().NotNullable()
                .WithColumn("client_id").AsAnsiString(50).NotNullable().Indexed("IX_Clients_ClientId")
                .WithColumn("client_secret").AsString(250).NotNullable();

            Create.Index("IX_Clients_Id")
                .OnTable("Clients")
                .OnColumn("id").Unique();

            Create.Index("IX_Clients_ClientId")
                .OnTable("Clients")
                .OnColumn("client_id");
        }

        public override void Down()
        {
            Delete.Table("Clients");
        }
    }
}