using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Project.Tech.Shop.Services.Common;
using Microsoft.Extensions.Logging;
using Project.Tech.Shop.Services.Products.Enitites;

namespace Project.Tech.Shop.Services.Products.Repositories;

/// <summary>
/// Service class implementation of a <see cref="IProductsRepository"/>
/// </summary>
public class ProductsRepository : IProductsRepository
{
    private readonly ProductsContext _context;
    private readonly ILogger<ProductsRepository> _logger;

    public ProductsRepository(ProductsContext context, ILogger<ProductsRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    ///<inheritdoc />
    public IUnitOfWork UnitOfWork => _context;

    public ProductsRepository(ProductsContext context) => _context = context ?? throw new ArgumentNullException(nameof(context));

    ///<inheritdoc />
    public async Task<Result<Product>> GetByIdAsync(int productId, CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .SingleOrDefaultAsync(p => p.ProductId == productId, cancellationToken);

        return product != null ? Result.Success(product) : Result.Failure<Product>("Product not found.");
    }

    ///<inheritdoc />
    public async Task<UnitResult<UserDbErrorReason>> AddAsync(Product product, CancellationToken cancellationToken)
    {
        if (product == null) throw new ArgumentNullException(nameof(product));

        await _context.Products.AddAsync(product, cancellationToken);
        return await _context.SaveEntitiesAsync(cancellationToken);
    }

    ///<inheritdoc />
    public async Task<UnitResult<UserDbErrorReason>> UpdateAsync(Product product, CancellationToken cancellationToken)
    {
        if (product == null) throw new ArgumentNullException(nameof(product));

        _context.Products.Update(product);
        return await _context.SaveEntitiesAsync(cancellationToken);
    }

    ///<inheritdoc />
    public async Task<UnitResult<UserDbErrorReason>> DeleteAsync(int productId, CancellationToken cancellationToken)
    {
        var product = _context.Products.Find(productId);
        if (product == null)
        {
            return UnitResult.Failure(UserDbErrorReason.NotFound);
        }

        _context.Products.Remove(product);
        return await _context.SaveEntitiesAsync(cancellationToken);
    }

    ///<inheritdoc />
    public async Task<Result<IReadOnlyCollection<Product>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var products = await _context.Products
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyCollection<Product>>(products);
    }

    ///<inheritdoc />
    public async Task<Result<IReadOnlyCollection<Product>>> ListByCategoryAsync(Category category, CancellationToken cancellationToken)
    {
        return await _context.Products
            .Where(p => p.Category == category)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public IQueryable<Product> GetAllQueryable()
    {
        return _context.Products.AsQueryable();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<string>> GetDistinctBrandsAsync()
    {
        return await _context.Products
            .Select(p => p.Brand)
            .Distinct()
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<(decimal minPrice, decimal maxPrice)> GetPriceRangeAsync()
    {
        var minPrice = await _context.Products.MinAsync(p => p.Price);
        var maxPrice = await _context.Products.MaxAsync(p => p.Price);
        return (minPrice, maxPrice);
    }

    /// <inheritdoc />
    public async Task<(int minStorage, int maxStorage)> GetStorageRangeAsync()
    {
        var minStorage = await _context.Products.MinAsync(p => p.Storage.HasValue ? p.Storage.Value : 0);
        var maxStorage = await _context.Products.MaxAsync(p => p.Storage.HasValue ? p.Storage.Value : 0);
        return (minStorage, maxStorage);
    }
}