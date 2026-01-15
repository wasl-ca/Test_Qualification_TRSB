public class EmailTest
{
    [Theory]
    [InlineData("user@example.com")]
    [InlineData("test@example.com")]
    public void Create_ValidEmail_Success(string emailString)
    {
        // Act
        var email = new Email(emailString);

        // Assert
        Assert.Equal(emailString, email.Address);
    }
    [Theory]
    [InlineData("user@.com")]
    [InlineData("userexample.com")]
    [InlineData("user@com")]
    [InlineData("@example.com")]
    public void Create_InvalidEmail_ThrowsArgumentException(string emailString)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Email(emailString));
    }
}