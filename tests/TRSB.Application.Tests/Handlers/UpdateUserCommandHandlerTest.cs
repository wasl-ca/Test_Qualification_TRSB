public class UpdateUserCommandHandlerTest
{
    [Fact]
    public async Task Handle_UpdateUserCommand_Success()
    {
        // Arrange
        var user = new User("User1", "User One", new Email("user1@example.com"));
        user.SetPasswordHash("Hash1");
        var seedUsers = new[] { user };
        var dbContext = new FakeRepository().CreateDbContext("UpdateUserCommand_Success", seedUsers);
        var userRepository = new EfRepository<User>(dbContext);
        var passwordPolicy = new Mock<IPasswordPolicy>(MockBehavior.Strict);
        var passwordHasher = new Mock<IPasswordHasher<User>>(MockBehavior.Strict);
        passwordPolicy.Setup(p => p.ValidateAsync(It.IsAny<string>())).ReturnsAsync(true);
        passwordHasher.Setup(p => p.HashPassword(It.IsAny<User>(), It.IsAny<string>())).Returns("new-hash");
        var handler = new UpdateUserCommandHandler(userRepository, passwordHasher.Object, passwordPolicy.Object);
        var command = new UpdateUserCommand(dbContext.Users.First().Id, "User1", "NewPassword123!");
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
        Assert.Equal("new-hash", dbContext.Users.First().PasswordHash);
    }


    [Fact]
    public async Task Handle_UpdateUserCommand_UserNotFound()
    {
        // Arrange
        var dbContext = new FakeRepository().CreateDbContext("UpdateUserCommand_UserNotFound", null);
        var userRepository = new EfRepository<User>(dbContext);
        var passwordPolicy = new Mock<IPasswordPolicy>(MockBehavior.Strict);
        var passwordHasher = new Mock<IPasswordHasher<User>>(MockBehavior.Strict);
        var handler = new UpdateUserCommandHandler(userRepository, passwordHasher.Object, passwordPolicy.Object);
        var command = new UpdateUserCommand(Guid.NewGuid(), "User1", "NewPassword123!");
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Utilisateur non trouvé.", result.Error);
    }

    [Fact]
    public async Task Handle_UpdateUserCommand_InvalidPassword()
    {
        // Arrange
        var user = new User("User1", "User One", new Email("user1@example.com"));
        user.SetPasswordHash("Hash1");
        var seedUsers = new[] { user };
        var dbContext = new FakeRepository().CreateDbContext("UpdateUserCommand_InvalidPassword", seedUsers);
        var userRepository = new EfRepository<User>(dbContext);
        var passwordPolicy = new Mock<IPasswordPolicy>(MockBehavior.Strict);
        var passwordHasher = new Mock<IPasswordHasher<User>>(MockBehavior.Strict);
        passwordPolicy.Setup(p => p.ValidateAsync(It.IsAny<string>())).ReturnsAsync(false);
        var handler = new UpdateUserCommandHandler(userRepository, passwordHasher.Object, passwordPolicy.Object
        );
        var command = new UpdateUserCommand(dbContext.Users.First().Id, "User1", "weak");
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Le mot de passe ne respecte pas la politique.", result.Error);
    }
} 
        