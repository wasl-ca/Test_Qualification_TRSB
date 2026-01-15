public class UsersControllerTests
{
    [Fact]
    public async Task CreateUser_ReturnsOk()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        mediatorMock.Setup(m => m.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Guid>.Success(Guid.NewGuid()));
        var controller = new UsersController(mediatorMock.Object);

        var request = new CreateUserCommand(
            "Test User",
            "testuser",
            "Password123!",
            "testuser@example.com"
        );
        // Act
        var result = await controller.Create(request);
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);

    }

    [Fact]
    public async Task CreateUser_ReturnsBadRequest_OnFailure()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        mediatorMock.Setup(m => m.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Guid>.Failure("User creation failed"));
        var controller = new UsersController(mediatorMock.Object);

        var request = new CreateUserCommand(
            "Test User",
            "testuser",
            "Password123!",
            "testuser@example.com"
        );
        // Act
        var result = await controller.Create(request);
        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);
    }

    [Fact]
    public async Task Login_ReturnsOk()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        mediatorMock.Setup(m => m.Send(It.IsAny<LoginUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<string>.Success("mocked-jwt-token"));
        var controller = new UsersController(mediatorMock.Object);
        var request = new LoginUserCommand(
            "testuser",
            "Password123!"
        );
        // Act
        var result = await controller.Login(request);
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_OnFailure()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        mediatorMock.Setup(m => m.Send(It.IsAny<LoginUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<string>.Failure("Login failed"));
        var controller = new UsersController(mediatorMock.Object);
        var request = new LoginUserCommand(
            "testuser",
            "Password123!"
        );
        // Act
        var result = await controller.Login(request);
        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.NotNull(unauthorizedResult.Value);
        Assert.Equal("Login failed", unauthorizedResult.Value);
    }

    [Fact]
    public async Task GetProfile_ReturnsOk()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        mediatorMock.Setup(m => m.Send(It.IsAny<GetUserProfileQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<UserProfileDto>.Success(
                new UserProfileDto
                {
                    Username = "testuser",
                    Name = "Test User",
                    Email = "testuser@example.com"
                }));

        var controller = new UsersController(mediatorMock.Object);
        MockUser(controller, Guid.NewGuid(), "testuser");
        // Act
        var result = await controller.GetProfile();
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GetProfile_ReturnsNotFound_OnFailure()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        mediatorMock.Setup(m => m.Send(It.IsAny<GetUserProfileQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<UserProfileDto>.Failure("User not found"));
        var controller = new UsersController(mediatorMock.Object);
        MockUser(controller, Guid.NewGuid(), "testuser");
        // Act
        var result = await controller.GetProfile();
        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.NotNull(notFoundResult.Value);
        Assert.Equal("User not found", notFoundResult.Value);
    }

    [Fact]
    public async Task UpdateProfile_ReturnsOk()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        mediatorMock.Setup(m => m.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));
        var controller = new UsersController(mediatorMock.Object);
        MockUser(controller, Guid.NewGuid(), "testuser");
        var request = new UpdateUserCommand(
            Guid.NewGuid(),
            "testuser",
            "UpdatePassword123!"
        );
        // Act
        var result = await controller.UpdateProfile(request);
        // Assert
        var okResult = Assert.IsType<OkResult>(result);
        Assert.NotNull(okResult);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task UpdateProfile_ReturnsBadRequest_OnFailure()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        mediatorMock.Setup(m => m.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Failure("Update failed"));
        var controller = new UsersController(mediatorMock.Object);
        MockUser(controller, Guid.NewGuid(), "testuser");
        var request = new UpdateUserCommand(
            Guid.NewGuid(),
            "testuser",
            "UpdatePassword123!"
        );
        // Act
        var result = await controller.UpdateProfile(request);
        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);
        Assert.Equal("Update failed", badRequestResult.Value);
    }

    private static void MockUser(ControllerBase controller, Guid userId, string username)
    {
        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        new Claim(ClaimTypes.Name, username)
    };
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims))
            }
        };
    }

}