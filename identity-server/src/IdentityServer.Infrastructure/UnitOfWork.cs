using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Infrastructure.Abstractions;
using IdentityServer.Infrastructure.Abstractions.Repositories;
using Npgsql;

namespace IdentityServer.Infrastructure
{
    public class UnitOfWork<T> : IUnitOfWork<T>
        where T : IRepository
    {
        private readonly NpgsqlConnection _connection;

        public UnitOfWork(NpgsqlConnection connection, T repository)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public T Repository { get; }

        private NpgsqlTransaction _transaction;
        
        public IDisposable BeginTransaction() 
            => _transaction = _connection.BeginTransaction();

        public async Task SaveAsync(CancellationToken cancellationToken)
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
            }
        }
    }
}