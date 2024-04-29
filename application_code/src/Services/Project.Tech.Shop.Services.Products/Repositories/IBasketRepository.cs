using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Project.Tech.Shop.Services.Products.Enitites;
using Project.Tech.Shop.Services.Common;

namespace Project.Tech.Shop.Services.Products.Repositories;

public interface IBasketRepository
{
    /// <summary>
    /// Unit of Work property to coordinate work with database  
    /// </summary>
    IUnitOfWork UnitOfWork { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="productId"></param>
    /// <param name="quantity"></param>
    /// <returns></returns>
    Task<Result> AddItemAsync(Guid customerId, int productId, int quantity);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    Task<Result<Basket>> GetActiveBasketByCustomerIdAsync(Guid customerId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="productId"></param>
    /// <param name="basketItemId"></param>
    /// <returns></returns>
    Task<Result> RemoveItemAsync(Guid customerId, int productId, int basketItemId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="productId"></param>
    /// <param name="quantity"></param>
    /// <returns></returns>
    Task<Result> UpdateItemQuantityAsync(Guid customerId, int productId, int quantity);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    Task<Result> ClearBasketAsync(Guid customerId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    Task<Result<int>> GetItemCountAsync(Guid customerId);
}