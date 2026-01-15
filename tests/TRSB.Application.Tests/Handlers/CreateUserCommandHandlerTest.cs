



public class CreateUserCommandHandlerTest
{
    [Fact]
    public async Task Handle_CreateUserCommand_Success()
    {
        // Arrange
        var dbContext = new FakeRepository().CreateDbContext("CreateUserCommand_Success");
        var userRepository = new EfRepository<User>(dbContext);
        var passwordPolicy = new Mock<IPasswordPolicy>(MockBehavior.Strict);
        var passwordHasher = new Mock<IPasswordHasher<User>>(MockBehavior.Strict);
        passwordPolicy.Setup(p => p.ValidateAsync(It.IsAny<string>())).ReturnsAsync(true);
        passwordHasher.Setup(p => p.HashPassword(It.IsAny<User>(), It.IsAny<string>())).Returns("hashed-password");

        // Act
        var handler = new CreateUserCommandHandler(userRepository, passwordPolicy.Object, passwordHasher.Object);
        var command = new CreateUserCommand("User1", "User One", "user1@example.com", "password");
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(1, dbContext.Users.Count());
        Assert.True(await dbContext.Users.AnyAsync(u => u.Username == "User1" && u.EmailValue == "user1@example.com"));
        Assert.NotEqual(Guid.Empty, result.Value);
        Assert.Equal(dbContext.Users.First().Id, result.Value);
        Assert.Equal("hashed-password", dbContext.Users.First().PasswordHash);
    }

    [Fact]
    public async Task Handle_CreateUserCommand_DuplicateUsername()
    {
        // Arrange
        var user1 = new User("User1", "User One", new Email("user1@example.com"));
        user1.SetPasswordHash("Hash1");
        var seedUsers = new[] { user1 };
        var dbContext = new FakeRepository().CreateDbContext("CreateUserCommand_DuplicateUsername",seedUsers);
        var userRepository = new EfRepository<User>(dbContext);
        var passwordPolicy = new Mock<IPasswordPolicy>(MockBehavior.Strict);
        var passwordHasher = new Mock<IPasswordHasher<User>>(MockBehavior.Strict);
        passwordPolicy.Setup(p => p.ValidateAsync(It.IsAny<string>())).ReturnsAsync(true);
        passwordHasher.Setup(p => p.HashPassword(It.IsAny<User>(), It.IsAny<string>())).Returns("hashed-password");
        
        // Act
        var handler = new CreateUserCommandHandler(userRepository, passwordPolicy.Object, passwordHasher.Object);
        var command = new CreateUserCommand("User1", "User Two", "user2@example.com", "password");
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Le nom d'usager existe déjà", result.Error);
        Assert.Equal(1, dbContext.Users.Count());
    }

    [Fact]
    public async Task Handle_CreateUserCommand_DuplicateEmail()
    {
        // Arrange
        var user1 = new User("User1", "User One", new Email("user1@example.com"));
        user1.SetPasswordHash("Hash1");
        var seedUsers = new[] { user1 };
        var dbContext = new FakeRepository().CreateDbContext("CreateUserCommand_DuplicateEmail", seedUsers);
        var userRepository = new EfRepository<User>(dbContext);
        var passwordPolicy = new Mock<IPasswordPolicy>(MockBehavior.Strict);
        var passwordHasher = new Mock<IPasswordHasher<User>>(MockBehavior.Strict);
        passwordPolicy.Setup(p => p.ValidateAsync(It.IsAny<string>())).ReturnsAsync(true);
        passwordHasher.Setup(p => p.HashPassword(It.IsAny<User>(), It.IsAny<string>())).Returns("hashed-password");

        // Act
        var handler = new CreateUserCommandHandler(userRepository, passwordPolicy.Object, passwordHasher.Object);
        var command = new CreateUserCommand("User2", "User Two", "user1@example.com", "password");
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Le courriel existe déjà", result.Error);
        Assert.Equal(1, dbContext.Users.Count());
    }

    [Fact]
    public async Task Handle_CreateUserCommand_MissingUsername()
    {
        // Arrange
        var dbContext = new FakeRepository().CreateDbContext("CreateUserCommand_MissingUsername");
        var userRepository = new EfRepository<User>(dbContext);
        var passwordPolicy = new Mock<IPasswordPolicy>(MockBehavior.Strict);
        var passwordHasher = new Mock<IPasswordHasher<User>>(MockBehavior.Strict);
        passwordPolicy.Setup(p => p.ValidateAsync(It.IsAny<string>())).ReturnsAsync(true);
        passwordHasher.Setup(p => p.HashPassword(It.IsAny<User>(), It.IsAny<string>())).Returns("hashed-password");
        
        // Act
        var handler = new CreateUserCommandHandler(userRepository, passwordPolicy.Object, passwordHasher.Object);
        var command = new CreateUserCommand("", "User Two", "user2@example.com", "password");
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Le nom d'usager est requis", result.Error);
        Assert.Equal(0, dbContext.Users.Count());
    }

    [Fact]
    public async Task Handle_CreateUserCommand_MissingPassword()
    {
        // Arrange
        var dbContext = new FakeRepository().CreateDbContext("CreateUserCommand_MissingPassword");
        var userRepository = new EfRepository<User>(dbContext);
        var passwordPolicy = new Mock<IPasswordPolicy>(MockBehavior.Strict);
        var passwordHasher = new Mock<IPasswordHasher<User>>(MockBehavior.Strict);
        passwordPolicy.Setup(p => p.ValidateAsync(It.IsAny<string>())).ReturnsAsync(true);
        passwordHasher.Setup(p => p.HashPassword(It.IsAny<User>(), It.IsAny<string>())).Returns("hashed-password");
        // Act
        var handler = new CreateUserCommandHandler(userRepository, passwordPolicy.Object, passwordHasher.Object);
        var command = new CreateUserCommand("User2", "User Two", "user2@example.com", "");
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Le mot de passe est requis", result.Error);
        Assert.Equal(0, dbContext.Users.Count());
    }

    [Fact]
    public async Task Handle_CreateUserCommand_MissingEmail()
    {
        // Arrange
        var dbContext = new FakeRepository().CreateDbContext("CreateUserCommand_MissingEmail");
        var userRepository = new EfRepository<User>(dbContext);
        var passwordPolicy = new Mock<IPasswordPolicy>(MockBehavior.Strict);
        var passwordHasher = new Mock<IPasswordHasher<User>>(MockBehavior.Strict);
        passwordPolicy.Setup(p => p.ValidateAsync(It.IsAny<string>())).ReturnsAsync(true);
        passwordHasher.Setup(p => p.HashPassword(It.IsAny<User>(), It.IsAny<string>())).Returns("hashed-password");

        // Act
        var handler = new CreateUserCommandHandler(userRepository, passwordPolicy.Object, passwordHasher.Object);
        var command = new CreateUserCommand("User2", "User Two", "", "password");
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Le courriel est requis", result.Error);
        Assert.Equal(0, dbContext.Users.Count());
    }

    [Fact]
    public async Task Handle_CreateUserCommand_InvalidEmail()
    {
        // Arrange
        var dbContext = new FakeRepository().CreateDbContext("CreateUserCommand_InvalidEmail");
        var userRepository = new EfRepository<User>(dbContext);
        var passwordPolicy = new Mock<IPasswordPolicy>(MockBehavior.Strict);
        var passwordHasher = new Mock<IPasswordHasher<User>>(MockBehavior.Strict);
        passwordPolicy.Setup(p => p.ValidateAsync(It.IsAny<string>())).ReturnsAsync(true);
        passwordHasher.Setup(p => p.HashPassword(It.IsAny<User>(), It.IsAny<string>())).Returns("hashed-password");

        // Act
        var handler = new CreateUserCommandHandler(userRepository, passwordPolicy.Object, passwordHasher.Object);
        var command = new CreateUserCommand("User2", "User Two", "Invalid-email", "password");
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Format de courriel est invalide", result.Error);
        Assert.Equal(0, dbContext.Users.Count());
    }

    [Fact]
    public async Task Handle_CreateUserCommand_MissingName()
    {
        // Arrange
        var dbContext = new FakeRepository().CreateDbContext("CreateUserCommand_MissingName");
        var userRepository = new EfRepository<User>(dbContext);
        var passwordPolicy = new Mock<IPasswordPolicy>(MockBehavior.Strict);
        var passwordHasher = new Mock<IPasswordHasher<User>>(MockBehavior.Strict);
        passwordPolicy.Setup(p => p.ValidateAsync(It.IsAny<string>())).ReturnsAsync(true);
        passwordHasher.Setup(p => p.HashPassword(It.IsAny<User>(), It.IsAny<string>())).Returns("hashed-password");
        
        // Act
        var handler = new CreateUserCommandHandler(userRepository, passwordPolicy.Object, passwordHasher.Object);
        var command = new CreateUserCommand("User2", "", "user2@example.com", "password");
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Le nom est requis", result.Error);
        Assert.Equal(0, dbContext.Users.Count());
    }

    [Fact]
    public async Task Handle_CreateUserCommand_InvalidPassword()
    {
        // Arrange
        var dbContext = new FakeRepository().CreateDbContext("CreateUserCommand_InvalidPassword");
        var userRepository = new EfRepository<User>(dbContext);
        var passwordPolicy = new Mock<IPasswordPolicy>(MockBehavior.Strict);
        var passwordHasher = new Mock<IPasswordHasher<User>>(MockBehavior.Strict);
        passwordPolicy.Setup(p => p.ValidateAsync(It.IsAny<string>())).ReturnsAsync(false);
        passwordHasher.Setup(p => p.HashPassword(It.IsAny<User>(), It.IsAny<string>())).Returns("hashed-password");

        // Act
        var handler = new CreateUserCommandHandler(userRepository, passwordPolicy.Object, passwordHasher.Object);
        var command = new CreateUserCommand("User2", "User Two", "user2@example.com", "password");
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Le mot de passe ne respecte pas les exigences de la politique", result.Error);
        Assert.Equal(0, dbContext.Users.Count());
    }
}