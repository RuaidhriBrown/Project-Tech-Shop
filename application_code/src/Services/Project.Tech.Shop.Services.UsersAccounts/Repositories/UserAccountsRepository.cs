using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Project.Tech.Shop.Services.UsersAccounts;
using Project.Tech.Shop.Services.Common;
using Project.Tech.Shop.Services.UsersAccounts.Entities;

namespace block.chain.services.Transactions.Repositories;

/// <summary>
/// Service class implementation of a <see cref="IUserAccountsRepository"/>
/// </summary>
public class UserAccountsRepository : IUserAccountsRepository
{
    private readonly UserAccountsContext _context;

    ///<inheritdoc />
    public IUnitOfWork UnitOfWork => _context;

    public UserAccountsRepository(UserAccountsContext context) => _context = context  ?? throw new ArgumentNullException(nameof(context));

    ///<inheritdoc />
    public async Task<Result<User>> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.Roles)
            .Include(u => u.Addresses)
            .Include(u => u.SecuritySettings)
            .Include(u => u.Preferences)
            .Include(u => u.Activities)
            .SingleOrDefaultAsync(u => u.UserId == userId, cancellationToken);

        return user != null ? Result.Success(user) : Result.Failure<User>("User not found.");
    }

    ///<inheritdoc />
    public async Task<Result<IReadOnlyCollection<User>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var users = await _context.Users
            .Include(u => u.Roles)
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
    public async Task<UnitResult<UserDbErrorReason>> AssignRoleToUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.Roles)
            .SingleOrDefaultAsync(u => u.UserId == userId, cancellationToken);

        if (user == null) return UnitResult.Failure(UserDbErrorReason.NotFound);

        var role = await _context.Roles.FindAsync(new object[] { roleId }, cancellationToken);
        if (role == null) return UnitResult.Failure(UserDbErrorReason.NotFound);

        user.Roles.Add(role);
        return await _context.SaveEntitiesAsync(cancellationToken);
    }

    ///<inheritdoc />
    public async Task<UnitResult<UserDbErrorReason>> RemoveRoleFromUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.Roles)
            .SingleOrDefaultAsync(u => u.UserId == userId, cancellationToken);

        if (user == null) return UnitResult.Failure(UserDbErrorReason.NotFound);

        var role = user.Roles.FirstOrDefault(r => r.RoleId == roleId);
        if (role == null) return UnitResult.Failure(UserDbErrorReason.NotFound);

        user.Roles.Remove(role);
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