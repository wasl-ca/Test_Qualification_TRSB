public class AuthenticationE2ETests
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public AuthenticationE2ETests()
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task RegisterUser_WithValidData_ShouldReturnSuccess()
    {
        // Arrange

        var registerRequest = new
        {
            Username = "testuser",
            Name = "Test User",
            Email = "testuser@example.com",
            Password = "StrongP@ssw0rd!"
        };
        // Act
        var response = await _client.PostAsJsonAsync("/api/users", registerRequest);
        // Assert
        var content = await response.Content.ReadAsStringAsync();
        var userId = JsonDocument.Parse(content)
            .RootElement.GetProperty("userId").GetGuid();
        Assert.True(userId != Guid.Empty, "UserId should not be empty");
        Assert.True(response.IsSuccessStatusCode, $"Response Status Code: {response.StatusCode}, Content: {content}");
    }

    [Fact]
    public async Task LoginUser_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var loginRequest = new
        {
            UsernameOrEmail = "testuser",
            Password = "StrongP@ssw0rd!"
        };
        // Act
        var response = await _client.PostAsJsonAsync("/api/users/login", loginRequest);
        // Assert
        var content = await response.Content.ReadAsStringAsync();
        var token = JsonDocument.Parse(content)
            .RootElement.GetProperty("token").GetString();
        Console.WriteLine("Token: " + token + "\nContent: " + content);
        Assert.False(string.IsNullOrEmpty(token), "Token should not be null or empty");
        Assert.True(response.IsSuccessStatusCode, $"Response Status Code: {response.StatusCode}, Content: {content}");
    }
}