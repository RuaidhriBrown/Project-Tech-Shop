using CSharpFunctionalExtensions;

namespace block.chain.services.Common;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task <UnitResult<UserDbErrorReason>> SaveEntitiesAsync(CancellationToken cancellationToken = default);
}