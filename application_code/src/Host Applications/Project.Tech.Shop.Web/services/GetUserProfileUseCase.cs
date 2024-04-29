using CSharpFunctionalExtensions;
using Project.Tech.Shop.Web.Models;
using Microsoft.Extensions.Logging;
using Project.Tech.Shop.Services.UsersAccounts.Repositories;

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
                LastName = userDetails.Surname
                // Map other required fields
            };

            return Result.Success(viewModel);
        }
    }
}
