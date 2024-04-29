using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Project.Tech.Shop.Services.Common;
using Microsoft.Extensions.Logging;
using Project.Tech.Shop.Services.Products.Enitites;

namespace Project.Tech.Shop.Services.Products.Repositories;

/// <summary>
/// Service class implementation of a <see cref="ISalesRepository"/>
/// </summary>
public class SalesRepository : ISalesRepository
{
    private readonly ProductsContext _context;
    private readonly ILogger<SalesRepository> _logger;

    public SalesRepository(ProductsContext context, ILogger<SalesRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    ///<inheritdoc />
    public IUnitOfWork UnitOfWork => _context;

    public SalesRepository(ProductsContext context) => _context = context ?? throw new ArgumentNullException(nameof(context));

    ///<inheritdoc />
    ///<inheritdoc />
    public async Task<Result<Sale>> GetByIdAsync(int saleId, CancellationToken cancellationToken)
    {
        var sale = await _context.Sales
            .Include(s => s.Basket)
                .ThenInclude(b => b.Items)
                .ThenInclude(i => i.Product)  // Make sure to load product details for each item
            .SingleOrDefaultAsync(s => s.SaleId == saleId, cancellationToken);

        return sale != null ? Result.Success(sale) : Result.Failure<Sale>("Sale not found.");
    }

    ///<inheritdoc />
    public async Task<Result<IReadOnlyCollection<Sale>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var sales = await _context.Sales
            .Include(s => s.Basket)
                .ThenInclude(b => b.Items)
                .ThenInclude(i => i.Product)  // Include product details for each basket item
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyCollection<Sale>>(sales);
    }

    ///<inheritdoc />
    public async Task<UnitResult<UserDbErrorReason>> AddAsync(Sale sale, CancellationToken cancellationToken)
    {
        if (sale == null) throw new ArgumentNullException(nameof(sale));

        await _context.Sales.AddAsync(sale, cancellationToken);
        return await _context.SaveEntitiesAsync(cancellationToken);
    }

    ///<inheritdoc />
    public async Task<UnitResult<UserDbErrorReason>> UpdateAsync(Sale sale, CancellationToken cancellationToken)
    {
        if (sale == null) throw new ArgumentNullException(nameof(sale));

        _context.Sales.Update(sale);
        return await _context.SaveEntitiesAsync(cancellationToken);
    }

    ///<inheritdoc />
    public async Task<Result<Sale>> CreateSaleFromBasketAsync(Guid basketId)
    {
        var basket = await _context.Baskets
            .Include(b => b.Items)
            .ThenInclude(i => i.Product)
            .SingleOrDefaultAsync(b => b.BasketId == basketId && b.Status == BasketStatus.Active);

        if (basket == null)
        {
            _logger.LogError("Basket not found for completion.");
            return Result.Failure<Sale>("Basket not found.");
        }

        var sale = new Sale
        {
            BasketId = basket.BasketId,
            CustomerId = basket.CustomerId,
            SaleDate = DateTime.UtcNow,
            TotalSaleAmount = basket.Items.Sum(item => item.Product.Price * item.Quantity),
            Status = "Completed"
        };

        _context.Sales.Add(sale);
        basket.Status = BasketStatus.Completed;  // Mark the basket as completed
        await _context.SaveChangesAsync();

        return Result.Success(sale);
    }

    ///<inheritdoc />
    public async Task<UnitResult<UserDbErrorReason>> DeleteAsync(int saleId, CancellationToken cancellationToken)
    {
        // This should not be used, cancelled orders should be kept for records
        var sale = _context.Sales.Find(saleId);
        if (sale == null)
        {
            return UnitResult.Failure(UserDbErrorReason.NotFound);
        }

        _context.Sales.Remove(sale);
        return await _context.SaveEntitiesAsync(cancellationToken);
    }

    ///<inheritdoc />
    public async Task<Result> CancelSaleAsync(int saleId)
    {
        var sale = await _context.Sales.FindAsync(saleId);
        if (sale == null)
        {
            _logger.LogError("Sale not found for cancellation.");
            return Result.Failure("Sale not found.");
        }

        // Optionally reset the linked basket status if needed
        var basket = await _context.Baskets.FindAsync(sale.BasketId);
        if (basket != null)
        {
            basket.Status = BasketStatus.Active;  // Re-activate the basket if cancellation is part of business logic
        }

        sale.Status = "Cancelled";
        await _context.SaveChangesAsync();

        return Result.Success();
    }

}