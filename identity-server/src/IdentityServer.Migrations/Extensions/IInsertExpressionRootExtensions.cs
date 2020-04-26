using System;
using System.Collections.Generic;
using System.Linq;

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

        public static void Role(this IInsertExpressionRoot insert, Guid id, string name, string displayName,
            string description,
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

        public static void Client(this IInsertExpressionRoot insert, Guid id, string name, string clientId,
            string clientSecret, bool isEnable, IEnumerable<Guid> resources,
            IEnumerable<Guid> roles, IEnumerable<Guid> permissions)
        {
            insert.IntoTable("Clients")
                .Row(new {id, name, is_active = isEnable, client_id = clientId, client_secret = clientSecret});

            foreach (var resource in resources)
            {
                insert.IntoTable("ClientsResources")
                    .Row(new { client_id = id, resource_id = resource});
            }

            foreach (var role in roles)
            {
                insert.IntoTable("ClientsRoles")
                    .Row(new { client_id = id, role_id = role});
            }
            
            foreach (var permission in permissions)
            {
                insert.IntoTable("ClientsPermissions")
                    .Row(new { client_id = id, permission_id = permission});
            }
        }
        
        public static void User(this IInsertExpressionRoot insert, Guid id, string mail, string password, bool isEnable,
            IEnumerable<Guid> roles = null, IEnumerable<Guid> permissions = null)
        {
            insert.IntoTable("Users")
                .Row(new {id, mail, is_active = isEnable, password});
            
            foreach (var role in roles ?? Enumerable.Empty<Guid>())
            {
                insert.IntoTable("UsersRoles")
                    .Row(new { user_id = id, role_id = role});
            }
            
            foreach (var permission in permissions ?? Enumerable.Empty<Guid>())
            {
                insert.IntoTable("UsersPermissions")
                    .Row(new { user_id = id, permission_id = permission});
            }
        }
    }
}