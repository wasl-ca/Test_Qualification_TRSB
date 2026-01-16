using System.ComponentModel.DataAnnotations;

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
        var registerRequest = new
        {
            Username = "testuser",
            Name = "Test User",
            Email = "testuser@example.com",
            Password = "StrongP@ssw0rd!"
        };
        await _client.PostAsJsonAsync("/api/users", registerRequest);

        var loginRequest = new
        {
            UsernameOrEmail = "testuser",
            Password = "StrongP@ssw0rd!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users/login", loginRequest);

        // Assert
        var content = await response.Content.ReadAsStringAsync();
        var token = JsonDocument.Parse(content).RootElement.GetProperty("token").GetString();
        Assert.False(string.IsNullOrEmpty(token), "Token should not be null or empty");
        Assert.True(response.IsSuccessStatusCode, $"Response Status Code: {response.StatusCode}, Content: {content}");
    }

    [Fact]
    public async Task GetUserProfile_AfterLogin_ShouldReturnProfile()
    {
        // Arrange - Créer un utilisateur
        var registerRequest = new
        {
            Username = "testuser",
            Name = "Test User",
            Email = "testuser@example.com",
            Password = "StrongP@ssw0rd!"
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/users", registerRequest);
        registerResponse.EnsureSuccessStatusCode();

        // Arrange - Se connecter
        var loginRequest = new
        {
            UsernameOrEmail = "testuser",
            Password = "StrongP@ssw0rd!"
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/users/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();

        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var loginJson = JsonDocument.Parse(loginContent);
        var token = loginJson.RootElement.GetProperty("token").GetString();

        Assert.NotNull(token);
        Assert.NotEmpty(token);

        // Act - Récupérer le profil avec le token
        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/api/users/profile");
        requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(requestMessage);

        // Debug - Afficher le contenu en cas d'erreur
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Status:  {response.StatusCode}");
            Console.WriteLine($"Error Content: {errorContent}");
        }

        // Assert - Vérifier le statut
        response.EnsureSuccessStatusCode();

        // Assert - Vérifier le contenu du profil
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Profile Content: {content}");

        var profile = JsonDocument.Parse(content).RootElement;
        var username = profile.GetProperty("username").GetString();
        var email = profile.GetProperty("email").GetString();
        var name = profile.GetProperty("name").GetString();

        Assert.Equal("testuser", username);
        Assert.Equal("testuser@example.com", email);
        Assert.Equal("Test User", name);
    }
}
        