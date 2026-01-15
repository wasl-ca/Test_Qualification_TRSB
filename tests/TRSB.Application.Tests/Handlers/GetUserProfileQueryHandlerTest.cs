

public class GetUserProfileQueryHandlerTest
{
    [Fact]
    public async Task Handle_GetUserProfileQuery_Success()
    {
        // Arrange
        var user = new User("User1", "User One", new Email("user1@example.com"));
        user.SetPasswordHash("Hash1");
        var seedUsers = new[] { user };
        var dbContext = new FakeRepository().CreateDbContext("GetUserProfile_Success", seedUsers);
        var userRepository = new EfRepository<User>(dbContext);
        var handler = new GetUserProfileQueryHandler(userRepository);
        var query = new GetUserProfileQuery(user.Id);
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("User1", result.Value.Username);
        Assert.Equal("User One", result.Value.Name);
        Assert.Equal("user1@example.com", result.Value.Email);
    }

    [Fact]
    public async Task Handle_GetUserProfileQuery_UserNotFound()
    {
        // Arrange
        var dbContext = new FakeRepository().CreateDbContext("GetUserProfile_UserNotFound");
        var userRepository = new EfRepository<User>(dbContext);
        var handler = new GetUserProfileQueryHandler(userRepository);
        var query = new GetUserProfileQuery(Guid.NewGuid());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Utilisateur non trouvé", result.Error);
    }
}