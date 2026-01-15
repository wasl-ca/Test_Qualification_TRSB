public class AuthenticationE2ETests
{
    private readonly CustomWebApplicationFactory _factory;

    public AuthenticationE2ETests()
    {
        _factory = new CustomWebApplicationFactory();
    }

    [Fact]
    public async Task RegisterUser_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();

        var registerRequest = new
        {
            Username = "testuser",
            Name = "Test User",
            Email = "testuser@example.com",
            Password = "StrongP@ssw0rd!"
        };
        // Act
        var response = await client.PostAsJsonAsync("/api/auth/register", registerRequest);
        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<Result<Guid>>();
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);
    }
}