using System;
using FluentMigrator.Runner.Generators.Postgres;
using FluentMigrator.Runner.Processors.Postgres;

namespace IdentityServer.Migrations
{
    public class CustomQuote : PostgresQuoter
    {
        public CustomQuote(PostgresOptions options) : base(options)
        {
        }

        private bool _isMini;
        public override string QuoteTableName(string tableName, string schemaName = null)
        {
            if (tableName.StartsWith("Mini"))
            {
                _isMini = true;
                return CreateSchemaPrefixedQuotedIdentifier(
                    QuoteSchemaName(schemaName), tableName);
            }

            _isMini = false;
            return base.QuoteTableName(tableName, schemaName);
        }

        public override string QuoteColumnName(string columnName)
        {
            if (columnName.StartsWith("Version"))
            {
                _isMini = false;
            }
            
            if (_isMini)
            {
                if (!columnName.Equals("User", StringComparison.InvariantCultureIgnoreCase))
                {
                    return columnName;
                }
            }
            
            return base.QuoteColumnName(columnName);
        }
    }
}