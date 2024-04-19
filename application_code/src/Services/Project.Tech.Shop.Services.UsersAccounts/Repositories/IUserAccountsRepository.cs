using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Project.Tech.Shop.Services.UsersAccounts;
using Project.Tech.Shop.Services.Common;
using Project.Tech.Shop.Services.UsersAccounts.Entities;

namespace block.chain.services.Transactions.Repositories;

public interface IUserAccountsRepository
{
    /// <summary>
    /// Unit of Work property to coordinate work with database  
    /// </summary>
    IUnitOfWork UnitOfWork { get; }

    // User-related methods
    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The user if found; otherwise, a failure result.</returns>
    Task<Result<User>> GetByIdAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="username"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result<User>> GetByUsernameAsync(string username, CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="username"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result<string>> GetPasswordHashByUsernameAsync(string username, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all users in the repository.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A read-only collection of users.</returns>
    Task<Result<IReadOnlyCollection<User>>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Adds a new user to the repository.
    /// </summary>
    /// <param name="user">The user entity to add.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A result indicating success or failure, including an error if applicable.</returns>
    Task<UnitResult<UserDbErrorReason>> AddUserAsync(User user, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing user in the repository.
    /// </summary>
    /// <param name="user">The user entity with updated values.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A result indicating success or failure, including an error if applicable.</returns>
    Task<UnitResult<UserDbErrorReason>> UpdateUserAsync(User user, CancellationToken cancellationToken);

    /// <summary>
    /// Removes a user from the repository by their unique identifier.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to remove.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A result indicating success or failure, including an error if applicable.</returns>
    Task<UnitResult<UserDbErrorReason>> RemoveUserAsync(Guid userId, CancellationToken cancellationToken);


    // Address-related methods
    /// <summary>
    /// Adds an address to a user's account.
    /// </summary>
    /// <param name="address">The address entity to add.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A result indicating success or failure, including an error if applicable.</returns>
    Task<UnitResult<UserDbErrorReason>> AddAddressToUserAsync(Address address, Guid userId, CancellationToken cancellationToken);


    // Role-related methods
    /// <summary>
    /// Assigns a role to a user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="roleId">The unique identifier of the role to assign.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A result indicating success or failure, including an error if applicable.</returns>
    Task<UnitResult<UserDbErrorReason>> AssignRoleToUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken);
    /// <summary>
    /// Removes a role from a user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="roleId">The unique identifier of the role to remove.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A result indicating success or failure, including an error if applicable.</returns>
    Task<UnitResult<UserDbErrorReason>> RemoveRoleFromUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken);


    // Security settings-related methods
    /// <summary>
    /// Updates the security settings for a user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="newSettings">The new security settings to apply.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A result indicating success or failure, including an error if applicable.</returns>
    Task<UnitResult<UserDbErrorReason>> UpdateSecuritySettingsAsync(Guid userId, SecuritySettings newSettings, CancellationToken cancellationToken);


    // User preferences-related methods
    /// <summary>
    /// Updates the user preferences for a user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="preferences">The new preferences to apply.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A result indicating success or failure, including an error if applicable.</returns>
    Task<UnitResult<UserDbErrorReason>> UpdateUserPreferencesAsync(Guid userId, UserPreferences preferences, CancellationToken cancellationToken);


    // Account activity-related methods
    /// <summary>
    /// Logs an account activity for a user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="activity">The account activity to log.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A result indicating success or failure, including an error if applicable.</returns>
    Task<UnitResult<UserDbErrorReason>> LogAccountActivityAsync(Guid userId, AccountActivity activity, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the list of account activities for a user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A result containing the list of account activities or failure.</returns>
    Task<Result<List<AccountActivity>>> GetUserActivitiesAsync(Guid userId, CancellationToken cancellationToken);

}