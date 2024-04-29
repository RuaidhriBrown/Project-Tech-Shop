using FluentAssertions;
using Project.Tech.Shop.Tests.Common;
using Project.Tech.Shop.Services.UsersAccounts.Entities;
using Project.Tech.Shop.Services.UsersAccounts.Repositories;

namespace Project.Tech.Shop.Services.UsersAccounts.Tests.Repositories;

public class UserAccountsRepositoryTests
{
    private readonly UserAccountsContext _userAccountContext;
    private readonly UserAccountsRepository _sut;

    public UserAccountsRepositoryTests()
    {
        _userAccountContext = TestUserAccountsContextDatabaseFactory.CreateDefaultTestContext();
        _sut = new UserAccountsRepository(_userAccountContext);
    }

    [Theory, AutoMoqData]
    public async Task ShouldReturnAllUsers(List<User> users)
    {
        //arrange
        _userAccountContext.Users.AddRange(users);
        await _userAccountContext.SaveEntitiesAsync(CancellationToken.None);

        var productToCheck = users[0];

        //act
        var result = await _sut.GetAllAsync(CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Count().Should().Be(users.Count());
        result.Value.First().Should().Be(productToCheck);
    }

    [Theory, AutoMoqData]
    public async Task ShouldReturnFailure_WhenUserNotFound(User userToCheck)
    {
        //act
        var result = await _sut.GetByIdAsync(userToCheck.UserId!, CancellationToken.None);

        //assert
        result.IsFailure.Should().BeTrue();
    }

    [Theory, AutoMoqData]
    public async Task ShouldAddUserSuccessfully(User user)
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        // Act
        var addResult = await _sut.AddUserAsync(user, cancellationToken);
        var result = await _sut.GetByIdAsync(user.UserId, cancellationToken);

        // Assert
        addResult.IsSuccess.Should().BeTrue("because the user should be added successfully");
        result.IsSuccess.Should().BeTrue("because the user should be retrievable after being added");
        result.Value.Should().BeEquivalentTo(user, "because the retrieved user should match the added user");
    }

    [Theory, AutoMoqData]
    public async Task ShouldUpdateUserSuccessfully(User user)
    {
        // Arrange
        await _sut.AddUserAsync(user, CancellationToken.None);
        user.FirstName = "Updated Name";

        // Act
        var updateResult = await _sut.UpdateUserAsync(user, CancellationToken.None);
        var updatedUser = await _sut.GetByIdAsync(user.UserId, CancellationToken.None);

        // Assert
        updateResult.IsSuccess.Should().BeTrue("because the update should succeed");
        updatedUser.Value.FirstName.Should().Be("Updated Name", "because the user first name should be updated");
    }

    [Theory, AutoMoqData]
    public async Task ShouldDeleteUserSuccessfully(User user)
    {
        // Arrange
        await _sut.AddUserAsync(user, CancellationToken.None);

        // Act
        var deleteResult = await _sut.RemoveUserAsync(user.UserId, CancellationToken.None);
        var result = await _sut.GetByIdAsync(user.UserId, CancellationToken.None);

        // Assert
        deleteResult.IsSuccess.Should().BeTrue("because the deletion should succeed");
        result.IsFailure.Should().BeTrue("because the user should no longer exist");
    }

    [Theory, AutoMoqData]
    public async Task ShouldReturnUsersByRole(List<User> users)
    {
        // Arrange
        foreach (var user in users)
        {
            await _sut.AddUserAsync(user, CancellationToken.None);
        }
        var expectedRole = users[0].Role;

        // Act
        var result = await _sut.GetAllUsersByRoleAsync(expectedRole, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.All(p => p.Role == expectedRole).Should().BeTrue("because all users should match the filtered role");
        result.Value.Should().ContainEquivalentOf(users[0], "because it includes at least one user from the setup");
    }

}
