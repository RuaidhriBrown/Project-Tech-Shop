using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Project.Tech.Shop.Services.Common;
using Project.Tech.Shop.Services.UsersAccounts.Entities;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Project.Tech.Shop.Services.UsersAccounts.Repositories;

/// <summary>
/// Service class implementation of a <see cref="IUserAccountsRepository"/>
/// </summary>
public class UserAccountsRepository : IUserAccountsRepository
{
    private readonly UserAccountsContext _context;

    public UserAccountsRepository(UserAccountsContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    ///<inheritdoc />
    public IUnitOfWork UnitOfWork => _context;

    ///<inheritdoc />
    public async Task<Result<User>> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.Addresses)
            .Include(u => u.SecuritySettings)
            .Include(u => u.Preferences)
            .Include(u => u.Activities)
            .SingleOrDefaultAsync(u => u.UserId == userId, cancellationToken);

        if (user == null)
        {
            return Result.Failure<User>("User not found.");
        }

        bool saveChanges = false;

        // Ensure SecuritySettings are not null and provide default values for non-nullable fields
        if (user.SecuritySettings == null)
        {
            user.SecuritySettings = new SecuritySettings
            {
                UserId = user.UserId,
                SecurityQuestion = "Default question",
                SecurityAnswerHash = "Default answer hash",
                TwoFactorEnabled = false // Assuming false as a default value
            };
            _context.SecuritySettings.Add(user.SecuritySettings);
            saveChanges = true;
        }

        // Ensure Preferences are not null and provide default values for non-nullable fields
        if (user.Preferences == null)
        {
            user.Preferences = new UserPreferences
            {
                UserId = user.UserId,
                ReceiveNewsletter = false, // Assuming false as a default value
                PreferredPaymentMethod = "None" // Assuming "None" as a default value
            };
            _context.Preferences.Add(user.Preferences);
            saveChanges = true;
        }

        if (saveChanges)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        return Result.Success(user);
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

        if (user.Preferences != null)
        {
            _context.Preferences.Update(user.Preferences);
        }

        if (user.SecuritySettings != null)
        {
            _context.SecuritySettings.Update(user.SecuritySettings);
        }

        var result = await _context.SaveEntitiesAsync(cancellationToken);

        if (result.IsSuccess)
        {
            await LogActivityAsync(user.UserId, "UpdateUser", "User profile updated.", cancellationToken);
        }

        return result;
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
        var result = await _context.SaveEntitiesAsync(cancellationToken);

        if (result.IsSuccess)
        {
            await LogActivityAsync(userId, "AddAddress", "Address added to user.", cancellationToken);
        }

        return result;
    }

    ///<inheritdoc />
    public async Task<UnitResult<UserDbErrorReason>> RemoveAddressFromUserAsync(Address address, Guid userId, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
        if (user == null)
        {
            return UnitResult.Failure(UserDbErrorReason.NotFound);
        }
        user.Addresses.Add(address);
        var result = await _context.SaveEntitiesAsync(cancellationToken);

        if (result.IsSuccess)
        {
            await LogActivityAsync(userId, "AddAddress", "Address added to user.", cancellationToken);
        }

        return result;
    }

    ///<inheritdoc />
    public async Task<UnitResult<UserDbErrorReason>> UpdateSecuritySettingsAsync(Guid userId, SecuritySettings newSettings, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.SecuritySettings)
            .SingleOrDefaultAsync(u => u.UserId == userId, cancellationToken);

        if (user == null) return UnitResult.Failure(UserDbErrorReason.NotFound);

        user.SecuritySettings = newSettings;
        var result = await _context.SaveEntitiesAsync(cancellationToken);

        if (result.IsSuccess)
        {
            await LogActivityAsync(userId, "UpdateSecuritySettings", "Security settings updated.", cancellationToken);
        }

        return result;
    }

    ///<inheritdoc />
    public async Task<UnitResult<UserDbErrorReason>> UpdateUserPreferencesAsync(Guid userId, UserPreferences preferences, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.Preferences)
            .SingleOrDefaultAsync(u => u.UserId == userId, cancellationToken);

        if (user == null) return UnitResult.Failure(UserDbErrorReason.NotFound);

        user.Preferences = preferences;
        var result = await _context.SaveEntitiesAsync(cancellationToken);

        if (result.IsSuccess)
        {
            await LogActivityAsync(userId, "UpdateUserPreferences", "User preferences updated.", cancellationToken);
        }

        return result;
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

    /// <summary>
    /// Logs an activity for a user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="activityType">The type of activity.</param>
    /// <param name="description">A description of the activity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task LogActivityAsync(Guid userId, string activityType, string description, CancellationToken cancellationToken)
    {
        var activity = new AccountActivity
        {
            UserId = userId,
            Timestamp = DateTime.UtcNow,
            ActivityType = activityType,
            Description = description
        };

        _context.Activities.Add(activity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
