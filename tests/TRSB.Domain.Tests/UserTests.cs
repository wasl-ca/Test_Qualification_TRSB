namespace TRSB.Domain.Tests;

public class UserTests
{
    [Fact]
    public void CreateUser_WithValidData_ShouldSucceed()
    {
        // Arrange
        var username = "asma123";
        var name = "Asma Elfaleh";
        var email = new Email("asma@example.com");

        // Act
        var user = new User(username, name, email);

        // Assert
        user.Username.Should().Be(username);
        user.Name.Should().Be(name);
        user.Email.Address.Should().Be(email.Address);
    }

    [Theory]
    [InlineData("user@.com")]
    [InlineData("userexample.com")]
    [InlineData("user@com")]
    [InlineData("@example.com")]
    public void CreateUser_WithInvalidEmail_ShouldThrowException(string invalidEmail)
    {
        // Arrange
        var username = "asma123";
        var name = "Asma Elfaleh";
        // Act & Assert
        Action act = () => new User(username, name, new Email(invalidEmail));
        act.Should().Throw<ArgumentException>()
            .WithMessage("Format de courriel est invalide");
    }

}
