using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Project.Tech.Shop.Services.Products.Enitites;
using Project.Tech.Shop.Services.Common;

namespace Project.Tech.Shop.Services.Products.Repositories;

public interface IProductsRepository
{
    /// <summary>
    /// Unit of Work property to coordinate work with database  
    /// </summary>
    IUnitOfWork UnitOfWork { get; }

    /// <summary>
    /// Retrieves a product by its ID.
    /// </summary>
    /// <param name="productId">The ID of the product to retrieve.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that results in the retrieval of a product.</returns>
    Task<Result<Product>> GetByIdAsync(int productId, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a new product to the repository.
    /// </summary>
    /// <param name="product">The product to add.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that results in a unit result indicating success or failure.</returns>
    Task<UnitResult<UserDbErrorReason>> AddAsync(Product product, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing product in the repository.
    /// </summary>
    /// <param name="product">The product to update.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that results in a unit result indicating success or failure.</returns>
    Task<UnitResult<UserDbErrorReason>> UpdateAsync(Product product, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a product from the repository based on its ID.
    /// </summary>
    /// <param name="productId">The ID of the product to delete.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that results in a unit result indicating success or failure.</returns>
    Task<UnitResult<UserDbErrorReason>> DeleteAsync(int productId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all products from the repository.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that results in a collection of products.</returns>
    Task<Result<IReadOnlyCollection<Product>>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a list of products filtered by category.
    /// </summary>
    /// <param name="category">The category to filter products by.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that results in a collection of products within the specified category.</returns>
    Task<Result<IReadOnlyCollection<Product>>> ListByCategoryAsync(Category category, CancellationToken cancellationToken);


    /// <summary>
    /// The GetAllQueryable() method as defined returns an IQueryable<Product>, which represents a query that has not yet been executed.
    /// The actual database query is not executed until you materialize the results with a method that forces execution
    /// </summary>
    /// <returns></returns>
    IQueryable<Product> GetAllQueryable();

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<string>> GetDistinctBrandsAsync();

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Task<(decimal minPrice, decimal maxPrice)> GetPriceRangeAsync();

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Task<(int minStorage, int maxStorage)> GetStorageRangeAsync();
}