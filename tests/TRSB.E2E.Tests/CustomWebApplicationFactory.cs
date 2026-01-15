public class CustomWebApplicationFactory
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove SQL Server DbContext
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<TRSBDbContext>));

            if (descriptor != null)
                services.Remove(descriptor);

            // SQLite InMemory
            services.AddDbContext<TRSBDbContext>(options =>
            {
                options.UseSqlite("DataSource=:memory:");
            });

            // Build service provider
            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TRSBDbContext>();

            db.Database.OpenConnection();   // IMPORTANT
            db.Database.EnsureCreated();    // IMPORTANT
        });
    }
}
