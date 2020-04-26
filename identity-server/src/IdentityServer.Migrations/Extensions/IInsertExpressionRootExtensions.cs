using System;
using System.Collections.Generic;

namespace FluentMigrator.Builders.Insert
{
    public static class IInsertExpressionRootExtensions
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
        
        public static void Role(this IInsertExpressionRoot insert, Guid id, string name, string displayName, string description,
            IEnumerable<Guid> permissions)
        {
            insert.IntoTable("Roles")
                .Row(new
                {
                    id,
                    name,
                    display_name = displayName,
                    description
                });

            foreach (var permission in permissions)
            {
                insert.IntoTable("RolesPermissions")
                    .Row(new
                    {
                        role_id = id,
                        permission_id = permission
                    });
            }
        }
        
        public static void Resource(this IInsertExpressionRoot insert, Guid id, string name, string displayName,
            string description, bool isEnable)
        {
            insert.IntoTable("Resources")
                .Row(new
                {
                    id,
                    name,
                    display_name = displayName,
                    description,
                    is_active = isEnable
                });
        }
    }
}