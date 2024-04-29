using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Project.Tech.Shop.Services.Common;
using Microsoft.Extensions.Logging;
using Project.Tech.Shop.Services.Products.Enitites;

namespace Project.Tech.Shop.Services.Products.Repositories;

/// <summary>
/// Service class implementation of a <see cref="IBasketRepository"/>
/// </summary>
public class BasketRepository : IBasketRepository
{
    private readonly ProductsContext _context;
    private readonly ILogger<BasketRepository> _logger;

    public BasketRepository(ProductsContext context, ILogger<BasketRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    ///<inheritdoc />
    public IUnitOfWork UnitOfWork => _context;

    public BasketRepository(ProductsContext context) => _context = context ?? throw new ArgumentNullException(nameof(context));

    ///<inheritdoc />
    public async Task<Result> AddItemAsync(Guid customerId, int productId, int quantity)
    {
        try
        {
            var basket = await _context.Baskets
                                      .Include(b => b.Items)
                                      .SingleOrDefaultAsync(b => b.CustomerId == customerId && b.Status == BasketStatus.Active);

            if (basket == null)
            {
                basket = new Basket { CustomerId = customerId };
                _context.Baskets.Add(basket);
            }

            var item = basket.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item == null)
            {
                item = new BasketItem { ProductId = productId, Quantity = quantity };
                basket.Items.Add(item);
            }
            else
            {
                item.Quantity += quantity;
            }

            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            // Log the exception here
            return Result.Failure($"Failed to add item: {ex.Message}");
        }
    }

    ///<inheritdoc />
    public async Task<Result<Basket>> GetActiveBasketByCustomerIdAsync(Guid customerId)
    {
        var activeBasket = await _context.Baskets
            .Include(b => b.Items)
            .ThenInclude(i => i.Product)
            .SingleOrDefaultAsync(b => b.CustomerId == customerId && b.Status == BasketStatus.Active);

        if (activeBasket == null)
        {
            activeBasket = new Basket { CustomerId = customerId, Status = BasketStatus.Active };
            _context.Baskets.Add(activeBasket);
            await _context.SaveChangesAsync();
        }

        return Result.Success(activeBasket);
    }

    ///<inheritdoc />
    public async Task<Result> RemoveItemAsync(Guid customerId, int productId, int basketItemId)
    {
        try
        {
            var basket = await _context.Baskets
                                       .Include(b => b.Items)
                                       .SingleOrDefaultAsync(b => b.CustomerId == customerId && b.Status == BasketStatus.Active);

            if (basket != null)
            {
                var item = basket.Items.FirstOrDefault(i => i.BasketItemId == basketItemId && i.ProductId == productId);
                if (item != null)
                {
                    // Decrement the quantity by one
                    item.Quantity -= 1;

                    // If the quantity drops to zero, remove the item completely
                    if (item.Quantity <= 0)
                    {
                        basket.Items.Remove(item);
                    }

                    await _context.SaveChangesAsync();
                    return Result.Success();
                }
            }
            return Result.Failure("Item not found in basket");
        }
        catch (Exception ex)
        {
            // Log the exception here
            return Result.Failure($"Failed to remove item: {ex.Message}");
        }
    }

    ///<inheritdoc />
    public async Task<Result> UpdateItemQuantityAsync(Guid customerId, int productId, int quantity)
    {
        try
        {
            var basket = await _context.Baskets
                                       .Include(b => b.Items)
                                       .SingleOrDefaultAsync(b => b.CustomerId == customerId && b.Status == BasketStatus.Active);

            if (basket != null)
            {
                var item = basket.Items.FirstOrDefault(i => i.ProductId == productId);
                if (item != null)
                {
                    item.Quantity = quantity;
                    await _context.SaveChangesAsync();
                    return Result.Success();
                }
            }
            return Result.Failure("Item not found in basket");
        }
        catch (Exception ex)
        {
            // Log the exception here
            return Result.Failure($"Failed to update item quantity: {ex.Message}");
        }
    }

    ///<inheritdoc />
    public async Task<Result> ClearBasketAsync(Guid customerId)
    {
        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                var basket = await _context.Baskets
                                           .Include(b => b.Items)
                                           .SingleOrDefaultAsync(b => b.CustomerId == customerId && b.Status == BasketStatus.Active);

                if (basket != null)
                {
                    _context.BasketItems.RemoveRange(basket.Items);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return Result.Success();
                }
                return Result.Failure("Basket not found");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError("Failed to clear basket: {Exception}", ex);
                return Result.Failure($"Failed to clear basket: {ex.Message}");
            }
        }
    }

    ///<inheritdoc />
    public async Task<Result<int>> GetItemCountAsync(Guid customerId)
    {
        try
        {
            var itemCount = await _context.Baskets
                .Where(b => b.CustomerId == customerId && b.Status == BasketStatus.Active)
                .SelectMany(b => b.Items)
                .SumAsync(i => i.Quantity);  // Summing up all the quantities of the items in the basket

            return Result.Success(itemCount);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to retrieve basket item count: {Exception}", ex);
            return Result.Failure<int>($"Failed to retrieve item count: {ex.Message}");
        }
    }

    public async Task<Result> CompleteBasketAsync(Guid basketId)
    {
        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                var basket = await _context.Baskets
                    .Include(b => b.Items)
                    .SingleOrDefaultAsync(b => b.BasketId == basketId && b.Status == BasketStatus.Active);

                if (basket == null)
                {
                    return Result.Failure("Basket not found.");
                }

                var totalSaleAmount = basket.Items.Sum(i => i.Quantity * i.Product.Price);

                var sale = new Sale
                {
                    CustomerId = basket.CustomerId,
                    SaleDate = DateTime.UtcNow,
                    TotalSaleAmount = totalSaleAmount,
                    BasketId = basket.BasketId,
                    Status = "Ordered"
                };

                _context.Sales.Add(sale);

                // Mark the basket as completed
                basket.Status = BasketStatus.Completed;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Result.Success();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError("Concurrency error occurred when trying to complete the basket: {Exception}", ex);
                return Result.Failure("A concurrency error occurred. Please try again.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError("An error occurred when trying to complete the basket: {Exception}", ex);
                return Result.Failure($"An error occurred: {ex.Message}");
            }
        }
    }


}