using System;
using FluentMigrator;

namespace FluentMigrator.Builders.Insert
{
    public static class ICreateExpressionRootExtensions
    {
        public static void Permission(this IInsertExpressionRoot insert, Guid id, string name, string displayName,
            string description)
        {
            insert.IntoTable("Permissions")
                .Row(new
                {
                    id,
                    name,
                    display_name = displayName,
                    description
                });
        }
    }
}