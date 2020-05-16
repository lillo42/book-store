using FluentMigrator;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations;
using Raven.Client.Exceptions;
using Raven.Client.Exceptions.Database;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;

namespace IdentityServer.Migrations.Migrations.RavenDb
{
    [Migration(20200515170000)]
    public class CreateDatabase : Migration
    {
        private readonly IDocumentStore _store;

        public CreateDatabase(IDocumentStore store)
        {
            _store = store;
        }

        public override void Up()
        {
            try
            {
                _store.Maintenance.ForDatabase(_store.Database).Send(new GetStatisticsOperation());
            }
            catch (DatabaseDoesNotExistException)
            {
                try
                {
                    _store.Maintenance.Server.Send(new CreateDatabaseOperation(new DatabaseRecord(_store.Database)));
                }
                catch (ConcurrencyException)
                {
                    // The database was already created before calling CreateDatabaseOperation
                }
            }
        }

        public override void Down()
        {
            _store.Maintenance.Server.Send(new DeleteDatabasesOperation(_store.Database, true));
        }
    }
}