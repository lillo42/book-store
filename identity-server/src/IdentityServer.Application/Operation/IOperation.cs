using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Domain.Abstractions;

namespace IdentityServer.Application.Operation
{
    public interface IOperation<T>
    {
        Task<Result> ExecuteAsync(T request, CancellationToken cancellationToken = default);
    }
}