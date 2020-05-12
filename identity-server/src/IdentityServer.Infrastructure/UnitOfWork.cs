using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Infrastructure.Abstractions;

namespace IdentityServer.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbFactory _factory;
        private DbConnection _connection;

        public UnitOfWork(IDbFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        private DbTransaction _transaction;

        public ValueTask<DbConnection> GetOrCreateDbConnection(CancellationToken cancellationToken = default)
        {
            if (_connection == null)
            {
                return new ValueTask<DbConnection>(CreateConnection(cancellationToken));
            }
            
            return new ValueTask<DbConnection>(_connection); 
        }

        private async Task<DbConnection> CreateConnection(CancellationToken cancellationToken)
        {
            return _connection = await CreateUnsafeConnection(cancellationToken)
                .ConfigureAwait(false);
        }
        
        public async Task<DbConnection> CreateUnsafeConnection(CancellationToken cancellationToken = default)
        {
            var connection = _factory.Create();
            
            await connection.OpenAsync(cancellationToken);

            return connection;
        }

        public async Task<IDisposable> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            return _transaction = await _connection
                .BeginTransactionAsync(isolationLevel)
                .ConfigureAwait(false);
        } 

        public async Task CommitAsync(CancellationToken cancellationToken)
        {
            if (_transaction != null)
            {
                await _transaction
                    .CommitAsync(cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        public void Dispose()
        {
            if (_connection?.State == ConnectionState.Open)
            {
                _connection.Close();
            }

            _connection?.Dispose();
            _transaction?.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            if (_connection != null )
            {
                if (_connection.State == ConnectionState.Open)
                {
                    await _connection.CloseAsync().ConfigureAwait(false);
                }
                
                await _connection.DisposeAsync().ConfigureAwait(false);    
            }

            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
            }
        }
    }
}