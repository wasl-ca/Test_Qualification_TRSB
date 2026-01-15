using TRSB.Infrastructure.Security;

namespace TRSB.Infrastructure.Tests;

public class ConfigurablePasswordPolicyTest
{
    private static IConfiguration BuildConfiguration(int minLength, int minSpecChars)
    {
        var InMemoryCollection = new Dictionary<string, string>
            {
                {"PasswordPolicy:MinLength", minLength.ToString()},
                {"PasswordPolicy:MinSpecialChars", minSpecChars.ToString()}
            };
        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddInMemoryCollection(InMemoryCollection);
        return configurationBuilder.Build();
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalse_WhenPasswordIsNull()
    {
        var config = BuildConfiguration(8, 2);

        var policy = new ConfigurablePasswordPolicy(config);

        var result = await policy.ValidateAsync(null!);

        Assert.False(result);
    }

    [Fact]
    public async Task ValidateAsync_ReturnsTrue_WhenPasswordIsValid()
    {
        var config = BuildConfiguration(minLength: 8, minSpecChars: 2);

        var policy = new ConfigurablePasswordPolicy(config);

        var result = await policy.ValidateAsync("Abc@#123");

        Assert.True(result);
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalse_WhenNotEnoughSpecialCharacters()
    {
        var config = BuildConfiguration(minLength: 6, minSpecChars: 2);

        var policy = new ConfigurablePasswordPolicy(config);

        var result = await policy.ValidateAsync("Abc@123");

        Assert.False(result);
    }
    [Fact]
    public async Task ValidateAsync_ReturnsFalse_WhenNotEnoughLength()
    {
        var config = BuildConfiguration(minLength: 8, minSpecChars: 2);

        var policy = new ConfigurablePasswordPolicy(config);

        var result = await policy.ValidateAsync("Abc@$1");

        Assert.False(result);
    }

}