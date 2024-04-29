using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Project.Tech.Shop.Services.UsersAccounts.Entities;
using Project.Tech.Shop.Services.UsersAccounts.Repositories;
using Project.Tech.Shop.Web.Models;

namespace Project.Tech.Shop.Web.services
{
    public class AccountUseCase
    {
        private readonly IUserAccountsRepository _userAccountsRepository;
        private readonly ILogger<AccountUseCase> _logger;

        public AccountUseCase(
            ILogger<AccountUseCase> logger,
            IUserAccountsRepository userAccounts)
        {
            _logger = logger;
            _userAccountsRepository = userAccounts;
        }

        public async Task<Result<bool>> AuthenticateUserAsync(string username, string password, CancellationToken cancellationToken)
        {
            var passwordHashResult = await _userAccountsRepository.GetPasswordHashByUsernameAsync(username, cancellationToken);

            if (passwordHashResult.IsFailure)
            {
                return Result.Failure<bool>("Could not find the user");
            }

            var verificationResult = BCrypt.Net.BCrypt.Verify(password, passwordHashResult.Value);
            if(verificationResult == false)
            {
                return Result.Failure<bool>("Password does not match");
            }

            return Result.Success(verificationResult);
        }

        internal async Task<Result<bool>> RegisterUserAsync(RegistrationViewModel userToRegister, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Registering new user");

            var userToAdd = new User()
            {
                Username = userToRegister.Username,
                Email = userToRegister.Email,
                FirstName = userToRegister.FirstName,
                Surname = userToRegister.LastName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userToRegister.Password)
            };

            var addUserResult = await _userAccountsRepository.AddUserAsync(userToAdd, cancellationToken);
            if (addUserResult.IsFailure)
            {
                _logger.LogError("Could not register the new user.");
                return Result.Failure<bool>("Failed to register user");
            }

            return Result.Success(true);
        }
    }
}
