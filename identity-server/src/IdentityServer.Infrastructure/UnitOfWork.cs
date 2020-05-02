using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Infrastructure.Abstractions;
using IdentityServer.Infrastructure.Abstractions.Repositories;

namespace IdentityServer.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbConnection _connection;

        public UnitOfWork(DbConnection connection, 
            IClientRepository clientRepository, 
            IPermissionRepository permissionRepository, 
            IResourceRepository resourceRepository, 
            IRoleRepository roleRepository, 
            IUserRepository userRepository)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            ClientRepository = clientRepository ?? throw new ArgumentNullException(nameof(clientRepository));
            PermissionRepository = permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
            ResourceRepository = resourceRepository ?? throw new ArgumentNullException(nameof(resourceRepository));
            RoleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        private DbTransaction _transaction;

        public IDisposable BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified) 
            => _transaction = _connection.BeginTransaction(isolationLevel);

        public async Task CommitAsync(CancellationToken cancellationToken)
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
            }
        }

        public IClientRepository ClientRepository { get; }

        public IPermissionRepository PermissionRepository { get; }

        public IResourceRepository ResourceRepository { get; }

        public IRoleRepository RoleRepository { get; }

        public IUserRepository UserRepository { get; }
    }
}