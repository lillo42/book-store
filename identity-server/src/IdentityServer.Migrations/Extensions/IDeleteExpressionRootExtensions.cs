using System;

namespace FluentMigrator.Builders.Delete
{
    public static class IDeleteExpressionRootExtensions
    {
        public static void Role(this IDeleteExpressionRoot delete, Guid id)
        {
            delete.FromTable("RolesPermissions")
                .Row(new {role_id = id});
            
            delete.FromTable("Roles")
                .Row(new {id});
        }
        
        public static void Permission(this IDeleteExpressionRoot delete, Guid id)
        {
            delete.FromTable("RolesPermissions")
                .Row(new {permission_id = id});

            delete.FromTable("UsersPermissions")
                .Row(new {permission_id = id});
            
            delete.FromTable("ClientsPermissions")
                .Row(new {permission_id = id});
            
            delete.FromTable("Permissions")
                .Row(new {id});
        }

        public static void Resource(this IDeleteExpressionRoot delete, Guid id)
        {
            delete.FromTable("ClientsResources")
                .Row(new {resource_id = id});
            
            delete.FromTable("Resource")
                .Row(new {id});
        }
        
        public static void Client(this IDeleteExpressionRoot delete, Guid id)
        {
            delete.FromTable("ClientsResources")
                .Row(new {client_id = id});
            
            delete.FromTable("ClientsRoles")
                .Row(new {client_id = id});
            
            delete.FromTable("ClientsPermissions")
                .Row(new {client_id = id});
            
            delete.FromTable("Clients")
                .Row(new {id});
        }
        
        public static void User(this IDeleteExpressionRoot delete, Guid id)
        {
            delete.FromTable("UsersPermissions")
                .Row(new {user_id = id});
            
            delete.FromTable("UsersRoles")
                .Row(new {user_id = id});

            delete.FromTable("Users")
                .Row(new {id});
        }
    }
}