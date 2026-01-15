




public class LoginUserCommandHandlerTest
{
    [Fact]
    public async Task Handle_LoginUserCommand_Success()
    {
        // Arrange
        var user = new User("User1", "User One", new Email("user1@example.com"));
        user.SetPasswordHash("Hash1");
        var seedUsers = new[] { user };
        var dbContext = new FakeRepository().CreateDbContext("LoginUserCommand_Success", seedUsers);
        var userRepository = new EfRepository<User>(dbContext);
        var configuration = new FakeConfiguration().CreateConfiguration();
        var passwordHasher = new Mock<IPasswordHasher<User>>(MockBehavior.Strict);
        passwordHasher.Setup(p => p.VerifyHashedPassword(It.IsAny<User>(), "Hash1", "Hash1"))
            .Returns(PasswordVerificationResult.Success);

        // Act
        var handler = new LoginUserCommandHandler(userRepository, configuration, passwordHasher.Object);
        var command = new LoginUserCommand("User1", "Hash1");
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task Handle_LoginUserCommand_InvalidPassword()
    {
        // Arrange
        var user = new User("User1", "User One", new Email("user1@example.com"));
        user.SetPasswordHash("Hash1");
        var seedUsers = new[] { user };
        var dbContext = new FakeRepository().CreateDbContext("LoginUserCommand_InvalidPassword", seedUsers);
        var userRepository = new EfRepository<User>(dbContext);
        var configuration = new FakeConfiguration().CreateConfiguration();
        var passwordHasher = new Mock<IPasswordHasher<User>>(MockBehavior.Strict);
        passwordHasher.Setup(p => p.VerifyHashedPassword(It.IsAny<User>(), "Hash1", "WrongPassword"))
            .Returns(PasswordVerificationResult.Failed);
        // Act
        var handler = new LoginUserCommandHandler(userRepository, configuration, passwordHasher.Object);
        var command = new LoginUserCommand("User1", "WrongPassword");
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Identifiants invalides", result.Error);

    }

    [Fact]
    public async Task Handle_LoginUserCommand_NonExistentUser()
    {
        // Arrange
        var user = new User("User1", "User One", new Email("user1@example.com"));
        user.SetPasswordHash("Hash1");
        var seedUsers = new[] { user };
        var dbContext = new FakeRepository().CreateDbContext("LoginUserCommand_NonExistentUser", seedUsers);
        var userRepository = new EfRepository<User>(dbContext);
        var configuration = new FakeConfiguration().CreateConfiguration();
        var passwordHasher = new Mock<IPasswordHasher<User>>(MockBehavior.Strict);
        // Act
        var handler = new LoginUserCommandHandler(userRepository, configuration, passwordHasher.Object);
        var command = new LoginUserCommand("NonExistentUser", "SomePassword");
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Identifiants invalides", result.Error);
    }

    [Fact]
    public async Task Handle_LoginUserCommand_MissingUsernameOrEmail()
    {
        // Arrange
        var dbContext = new FakeRepository().CreateDbContext("LoginUserCommand_MissingUsernameOrEmail", null);
        var userRepository = new EfRepository<User>(dbContext);
        var configuration = new FakeConfiguration().CreateConfiguration();
        var passwordHasher = new Mock<IPasswordHasher<User>>(MockBehavior.Strict);
        // Act
        var handler = new LoginUserCommandHandler(userRepository, configuration, passwordHasher.Object);
        var command = new LoginUserCommand("", "SomePassword");
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Le nom d'usager ou le courriel est requis", result.Error);
    }

    [Fact]
    public async Task Handle_LoginUserCommand_MissingPassword()
    {
        // Arrange
        var dbContext = new FakeRepository().CreateDbContext("LoginUserCommand_MissingPassword", null);
        var userRepository = new EfRepository<User>(dbContext);
        var configuration = new FakeConfiguration().CreateConfiguration();
        var passwordHasher = new Mock<IPasswordHasher<User>>(MockBehavior.Strict);
        // Act
        var handler = new LoginUserCommandHandler(userRepository, configuration, passwordHasher.Object);
        var command = new LoginUserCommand("SomeUser", "");
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Le mot de passe est requis", result.Error);
    }
}