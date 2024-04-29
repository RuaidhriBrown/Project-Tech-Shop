using CSharpFunctionalExtensions;
using Project.Tech.Shop.Services.Common;
using Project.Tech.Shop.Services.Products.Enitites;

public interface ISalesRepository
{
    /// <summary>
    /// Unit of Work property to coordinate work with the database. It provides access to repository-wide transactional operations.
    /// </summary>
    IUnitOfWork UnitOfWork { get; }

    /// <summary>
    /// Retrieves a sale by its unique identifier asynchronously.
    /// </summary>
    /// <param name="saleId">The identifier of the sale to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token for the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation and contains the result of finding the sale. Returns a successful result with the sale if found, otherwise returns a failure result.</returns>
    Task<Result<Sale>> GetByIdAsync(int saleId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all sales from the repository asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation and contains a collection of all sales.</returns>
    Task<Result<IReadOnlyCollection<Sale>>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Adds a new sale to the repository asynchronously.
    /// </summary>
    /// <param name="sale">The sale entity to add.</param>
    /// <param name="cancellationToken">Cancellation token for the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation, returning a result that indicates the success or failure of the add operation along with a specific error reason if failed.</returns>
    Task<UnitResult<UserDbErrorReason>> AddAsync(Sale sale, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing sale in the repository asynchronously.
    /// </summary>
    /// <param name="sale">The sale entity to update.</param>
    /// <param name="cancellationToken">Cancellation token for the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation, returning a result that indicates the success or failure of the update operation along with a specific error reason if failed.</returns>
    Task<UnitResult<UserDbErrorReason>> UpdateAsync(Sale sale, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a sale by its identifier from the repository asynchronously. Note: Deletion might not be advisable for record keeping.
    /// </summary>
    /// <param name="saleId">The identifier of the sale to delete.</param>
    /// <param name="cancellationToken">Cancellation token for the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation, returning a result that indicates the success or failure of the deletion operation along with a specific error reason if failed.</returns>
    Task<UnitResult<UserDbErrorReason>> DeleteAsync(int saleId, CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="basketId"></param>
    /// <returns></returns>
    Task<Result<Sale>> CreateSaleFromBasketAsync(Guid basketId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="saleId"></param>
    /// <returns></returns>
    Task<Result> CancelSaleAsync(int saleId);
}