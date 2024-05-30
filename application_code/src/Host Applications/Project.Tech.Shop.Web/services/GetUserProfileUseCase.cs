using CSharpFunctionalExtensions;
using Project.Tech.Shop.Web.Models;
using Microsoft.Extensions.Logging;
using Project.Tech.Shop.Services.UsersAccounts.Repositories;
using Project.Tech.Shop.Services.UsersAccounts.Entities;
using Microsoft.EntityFrameworkCore;

namespace Project.Tech.Shop.Web.services
{
    public class GetUserProfileUseCase
    {
        private readonly IUserAccountsRepository _userAccountsRepository;
        private readonly ILogger<GetUserProfileUseCase> _logger;

        public GetUserProfileUseCase(IUserAccountsRepository userAccounts,
            ILogger<GetUserProfileUseCase> logger)
        {
            _userAccountsRepository = userAccounts;
            _logger = logger;
        }

        public async Task<Result<UserProfileViewModel>> GetUserProfileByUsernameAsync(string username, CancellationToken cancellationToken)
        {
            var userDetailsResults = await _userAccountsRepository.GetByUsernameAsync(username, cancellationToken);

            if (userDetailsResults.IsFailure)
            {
                return Result.Failure<UserProfileViewModel>("Could not retrieve user profile details");
            }

            var userDetails = userDetailsResults.Value;

            var viewModel = new UserProfileViewModel
            {
                UserId = userDetails.UserId,
                Username = userDetails.Username,
                Email = userDetails.Email,
                FirstName = userDetails.FirstName,
                LastName = userDetails.Surname,
                Role = userDetails.Role.ToString(),
                TwoFactorEnabled = userDetails.SecuritySettings?.TwoFactorEnabled ?? false,
                Addresses = userDetails.Addresses.Select(a => new AddressViewModel
                {
                    AddressId = a.AddressId,
                    AddressLine = a.AddressLine,
                    City = a.City,
                    County = a.County,
                    PostCode = a.PostCode,
                    Country = a.Country,
                    IsShippingAddress = a.IsShippingAddress,
                    IsBillingAddress = a.IsBillingAddress
                }).ToList(),
                SecuritySettings = userDetails.SecuritySettings != null ? new SecuritySettingsViewModel
                {
                    TwoFactorEnabled = userDetails.SecuritySettings.TwoFactorEnabled,
                    SecurityQuestion = userDetails.SecuritySettings.SecurityQuestion,
                    SecurityAnswerHash = userDetails.SecuritySettings.SecurityAnswerHash
                } : new SecuritySettingsViewModel(),
                Preferences = userDetails.Preferences != null ? new UserPreferencesViewModel
                {
                    ReceiveNewsletter = userDetails.Preferences.ReceiveNewsletter,
                    PreferredPaymentMethod = userDetails.Preferences.PreferredPaymentMethod
                } : new UserPreferencesViewModel(),
                Activities = userDetails.Activities.Select(activity => new AccountActivityViewModel
                {
                    ActivityId = activity.ActivityId,
                    Timestamp = activity.Timestamp,
                    ActivityType = activity.ActivityType,
                    Description = activity.Description
                }).ToList()
            };

            if (userDetails.Role == Role.Admin)
            {
                viewModel.Role = "Admin";
            }
            else if (userDetails.Role == Role.Customer)
            {
                viewModel.Role = "Customer";
            }
            else if (userDetails.Role == Role.Support)
            {
                viewModel.Role = "Support";
            }

            return Result.Success(viewModel);
        }

        public async Task<Result> UpdateUserProfileAsync(UserProfileViewModel model, CancellationToken cancellationToken)
        {
            var userResult = await _userAccountsRepository.GetByIdAsync(model.UserId, cancellationToken);
            if (userResult.IsFailure)
            {
                _logger.LogError("User not found: {UserId}", model.UserId);
                return Result.Failure($"User not found");
            }

            var user = userResult.Value;

            // Update basic info
            user.Username = model.Username;
            user.FirstName = model.FirstName;
            user.Surname = model.LastName;
            user.Email = model.Email;

            // Update addresses
            foreach (var addressModel in model.Addresses)
            {
                var address = user.Addresses.FirstOrDefault(a => a.AddressId == addressModel.AddressId);
                if (address != null)
                {
                    address.AddressLine = addressModel.AddressLine ?? string.Empty;
                    address.City = addressModel.City ?? string.Empty;
                    address.County = addressModel.County ?? string.Empty;
                    address.PostCode = addressModel.PostCode ?? string.Empty;
                    address.Country = addressModel.Country ?? string.Empty;
                    address.IsShippingAddress = addressModel.IsShippingAddress;
                    address.IsBillingAddress = addressModel.IsBillingAddress;
                }
                else
                {
                    user.Addresses.Add(new Address
                    {
                        AddressId = Guid.NewGuid(),
                        UserId = user.UserId,
                        AddressLine = addressModel.AddressLine ?? string.Empty,
                        City = addressModel.City ?? string.Empty,
                        County = addressModel.County ?? string.Empty,
                        PostCode = addressModel.PostCode ?? string.Empty,
                        Country = addressModel.Country ?? string.Empty,
                        IsShippingAddress = addressModel.IsShippingAddress,
                        IsBillingAddress = addressModel.IsBillingAddress,
                        User = user
                    });
                }
            }

            // Handle removal of addresses if necessary, not shown here for brevity

            // Update security settings
            if (user.SecuritySettings != null)
            {
                user.SecuritySettings.TwoFactorEnabled = false;
                user.SecuritySettings.SecurityQuestion = string.Empty;
                user.SecuritySettings.SecurityAnswerHash = string.Empty;
            }

            // Update Preferences settings
            if (user.Preferences != null)
            {
                user.Preferences.ReceiveNewsletter = false;
                user.Preferences.PreferredPaymentMethod = string.Empty;
            }

            try
            {
                var updateResult = await _userAccountsRepository.UpdateUserAsync(user, cancellationToken);
                if (updateResult.IsFailure)
                {
                    return Result.Failure("Could not update user profile details");
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "A concurrency error occurred while updating user profile: {UserId}", user.UserId);
                return Result.Failure("The profile was modified by another user. Please reload the profile and try again.");
            }

            return Result.Success();
        }
    }
}
