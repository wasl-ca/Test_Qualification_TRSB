
namespace TRSB.Application.Tests
{
    public class FakeRepository
    {

        public TRSBDbContext CreateDbContext(string testName,User[]? seedUsers = null)
        {
            var options = new DbContextOptionsBuilder<TRSBDbContext>()
                .UseInMemoryDatabase(databaseName: "TRSB" + testName)
                .Options;
            var dbContext = new TRSBDbContext(options);
            foreach (var user in seedUsers ?? Array.Empty<User>())
            {
                dbContext.Users.Add(user);
                dbContext.SaveChanges();
            }
            
            return dbContext;
        }
    }

    public class FakeConfiguration
    {
        public IConfiguration CreateConfiguration()
        {
            var InMemoryCollection = new Dictionary<string, string>
            {
                {"Jwt:SigningKey", "ThisIsASecretKeyForJwtTokenGeneration"},
                {"Jwt:ExpireMinutes", "60"}
            };
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(InMemoryCollection);
            return configurationBuilder.Build();
        }
    }
}