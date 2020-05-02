using FluentMigrator;

namespace IdentityServer.Migrations.Migrations
{
    [Migration(20200403212300)]
    public class AddUsers : Migration
    {
        public override void Up()
        {
            Create.Table("Users")
                .WithColumn("id").AsGuid().PrimaryKey("PK_Users")
                .WithColumn("mail").AsString(100).NotNullable()
                .WithColumn("password").AsCustom("text").NotNullable()
                .WithColumn("is_active").AsBoolean().NotNullable();

            Create.Index("IX_User_Mail_Password")
                .OnTable("Users")
                .OnColumn("mail").Ascending()
                .OnColumn("password").Ascending();
        }

        public override void Down()
        {
            Delete.Table("Users");
        }
    }
}