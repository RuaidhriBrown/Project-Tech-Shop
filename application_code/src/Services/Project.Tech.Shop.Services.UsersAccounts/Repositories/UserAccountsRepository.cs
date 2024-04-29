using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Project.Tech.Shop.Services.Common;
using Project.Tech.Shop.Services.UsersAccounts.Entities;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography.X509Certificates;

namespace Project.Tech.Shop.Services.UsersAccounts.Repositories;

/// <summary>
/// Service class implementation of a <see cref="IUserAccountsRepository"/>
/// </summary>
public class UserAccountsRepository : IUserAccountsRepository
{
    private readonly UserAccountsContext _context;
    private readonly ILogger<UserAccountsRepository> _logger;

    public UserAccountsRepository(UserAccountsContext context, ILogger<UserAccountsRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    ///<inheritdoc />
    public IUnitOfWork UnitOfWork => _context;

    public UserAccountsRepository(UserAccountsContext context) => _context = context ?? throw new ArgumentNullException(nameof(context));

    ///<inheritdoc />
    public async Task<Result<User>> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.Addresses)
            .Include(u => u.SecuritySettings)
            .Include(u => u.Preferences)
            .Include(u => u.Activities)
            .SingleOrDefaultAsync(u => u.UserId == userId, cancellationToken);

        return user != null ? Result.Success(user) : Result.Failure<User>("User not found.");
    }


    ///<inheritdoc />
    public async Task<Result<User>> GetByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.Addresses)
            .Include(u => u.SecuritySettings)
            .Include(u => u.Preferences)
            .Include(u => u.Activities)
            .SingleOrDefaultAsync(u => u.Username == username, cancellationToken);

        return user != null ? Result.Success(user) : Result.Failure<User>("User not found.");
    }

    ///<inheritdoc />
    public async Task<Result<string>> GetPasswordHashByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        // Ensuring the username is not null or empty
        if (string.IsNullOrWhiteSpace(username))
        {
            return Result.Failure<string>("Username cannot be empty.");
        }

        var user = await _context.Users
            .SingleOrDefaultAsync(u => u.Username == username, cancellationToken);

        if (user == null)
        {
            // Optionally log the fact that no user was found
            _logger?.LogWarning("No user found with username: {Username}", username);
            return Result.Failure<string>("User not found.");
        }

        return Result.Success(user.PasswordHash);
    }

    ///<inheritdoc />
    public async Task<Result<IReadOnlyCollection<User>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var users = await _context.Users
            .Include(u => u.Addresses)
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyCollection<User>>(users);
    }

    ///<inheritdoc />
    public async Task<Result<IReadOnlyCollection<User>>> GetAllUsersByRoleAsync(Role role, CancellationToken cancellationToken)
    {
        var users = await _context.Users
            .Where(u => u.Role == role)
            .Include(u => u.Addresses)
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyCollection<User>>(users);
    }

    ///<inheritdoc />
    public async Task<UnitResult<UserDbErrorReason>> AddUserAsync(User user, CancellationToken cancellationToken)
    {
        await _context.Users.AddAsync(user, cancellationToken);
        return await _context.SaveEntitiesAsync(cancellationToken);
    }



    ///<inheritdoc />
    public async Task<UnitResult<UserDbErrorReason>> UpdateUserAsync(User user, CancellationToken cancellationToken)
    {
        _context.Users.Update(user);
        return await _context.SaveEntitiesAsync(cancellationToken);
    }

    ///<inheritdoc />
    public async Task<UnitResult<UserDbErrorReason>> RemoveUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);

        if (user == null)
        {
            return UnitResult.Failure(UserDbErrorReason.NotFound);
        }

        _context.Users.Remove(user);
        return await _context.SaveEntitiesAsync(cancellationToken);
    }

    ///<inheritdoc />
    public async Task<UnitResult<UserDbErrorReason>> AddAddressToUserAsync(Address address, Guid userId, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
        if (user == null)
        {
            return UnitResult.Failure(UserDbErrorReason.NotFound);
        }
        user.Addresses.Add(address);
        return await _context.SaveEntitiesAsync(cancellationToken);
    }

    ///<inheritdoc />
    public async Task<UnitResult<UserDbErrorReason>> UpdateSecuritySettingsAsync(Guid userId, SecuritySettings newSettings, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.SecuritySettings)
            .SingleOrDefaultAsync(u => u.UserId == userId, cancellationToken);

        if (user == null) return UnitResult.Failure(UserDbErrorReason.NotFound);

        user.SecuritySettings = newSettings;
        return await _context.SaveEntitiesAsync(cancellationToken);
    }

    ///<inheritdoc />
    public async Task<UnitResult<UserDbErrorReason>> UpdateUserPreferencesAsync(Guid userId, UserPreferences preferences, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.Preferences)
            .SingleOrDefaultAsync(u => u.UserId == userId, cancellationToken);

        if (user == null) return UnitResult.Failure(UserDbErrorReason.NotFound);

        user.Preferences = preferences;
        return await _context.SaveEntitiesAsync(cancellationToken);
    }

    ///<inheritdoc />
    public async Task<UnitResult<UserDbErrorReason>> LogAccountActivityAsync(Guid userId, AccountActivity activity, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.Activities)
            .SingleOrDefaultAsync(u => u.UserId == userId, cancellationToken);

        if (user == null) return UnitResult.Failure(UserDbErrorReason.NotFound);

        user.Activities.Add(activity);
        return await _context.SaveEntitiesAsync(cancellationToken);
    }

    ///<inheritdoc />
    public async Task<Result<List<AccountActivity>>> GetUserActivitiesAsync(Guid userId, CancellationToken cancellationToken)
    {
        var activities = await _context.Activities
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync(cancellationToken);

        return Result.Success(activities);
    }
}