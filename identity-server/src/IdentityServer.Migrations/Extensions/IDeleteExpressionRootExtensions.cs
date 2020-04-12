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
    }
}