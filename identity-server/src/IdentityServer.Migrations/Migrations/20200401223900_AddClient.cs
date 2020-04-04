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
                .WithColumn("client_id").AsAnsiString(50).NotNullable().Indexed("IX_Clients_ClientId")
                .WithColumn("password").AsString(250).NotNullable();
            
        }

        public override void Down()
        {
            Delete.Table("Clients");
        }
    }
}