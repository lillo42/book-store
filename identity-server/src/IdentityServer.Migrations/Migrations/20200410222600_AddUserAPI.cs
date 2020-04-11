using System;
using FluentMigrator;
using IdentityServer.Domain.Abstractions.Permission;
using IdentityServer.Domain.Abstractions.Role;
using IdentityServer.Domain.Common;
using Npgsql;

namespace IdentityServer.Migrations.Migrations
{
    [Migration(20200410222600)]
    public class AddUserAPI : Migration
    {
        private readonly NpgsqlConnection _connection;
        private readonly IPermissionAggregationStore _permissionAggregationStore;
        private readonly IRoleAggregationStore _roleAggregationStore;

        public AddUserAPI(
            NpgsqlConnection connection,
            IRoleAggregationStore roleAggregationStore, 
            IPermissionAggregationStore permissionAggregationStore)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _roleAggregationStore = roleAggregationStore ?? throw new ArgumentNullException(nameof(roleAggregationStore));
            _permissionAggregationStore = permissionAggregationStore ?? throw new ArgumentNullException(nameof(permissionAggregationStore));
        }

        public override void Up()
        {
            using (var trans = _connection.BeginTransaction())
            {
                var reader = _permissionAggregationStore.Create();
                reader.Create("read_user", "Read User", "Read User");
                _permissionAggregationStore.SaveAsync(reader);

                var write = _permissionAggregationStore.Create();
                write.Create("write_user", "Update User", "Update User");
                _permissionAggregationStore.SaveAsync(write);

                var create = _permissionAggregationStore.Create();
                create.Create("create_user", "Create User", "Create User");
                _permissionAggregationStore.SaveAsync(create);

                var delete = _permissionAggregationStore.Create();
                delete.Create("delete_user", "Delete User", "Delete User");
                _permissionAggregationStore.SaveAsync(delete);

                var manager = _roleAggregationStore.Create();
                manager.Create("manager_user", "Manger User", "Manager user");
                manager.AddPermission((Permission) reader.State);
                manager.AddPermission((Permission) write.State);
                manager.AddPermission((Permission) create.State);
                manager.AddPermission((Permission) delete.State);
                _roleAggregationStore.SaveAsync(manager);
                
             
                trans.Commit();   
            }
        }

        public override void Down()
        {
            throw new System.NotImplementedException();
        }
    }
}