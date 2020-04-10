using System.Threading;
using System.Threading.Tasks;

namespace IdentityServer.Infrastructure.Abstractions.Repositories
{
    public interface IRepository<T> : IRepository
    {
        Task CreateAsync(T entity, CancellationToken cancellationToken = default);
        
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

        Task IRepository.CreateAsync(object entity, CancellationToken cancellationToken)
            => CreateAsync((T) entity, cancellationToken);
        
        Task IRepository.UpdateAsync(object entity, CancellationToken cancellationToken)
            => CreateAsync((T) entity, cancellationToken);
        
        Task IRepository.DeleteAsync(object entity, CancellationToken cancellationToken)
            => CreateAsync((T) entity, cancellationToken);
    }

    public interface IRepository
    {
        Task CreateAsync(object entity, CancellationToken cancellationToken = default);
        
        Task UpdateAsync(object entity, CancellationToken cancellationToken = default);
        
        Task DeleteAsync(object entity, CancellationToken cancellationToken = default);
    }
}