// src/TRSB.Infrastructure/Data/TRSBDbContext.cs
using Microsoft.EntityFrameworkCore;
using TRSB.Domain.Entities;

namespace TRSB.Infrastructure.Data
{
    public class TRSBDbContext : DbContext
    {
        public TRSBDbContext(DbContextOptions<TRSBDbContext> options)
            : base(options)
        {
        }

        // DbSets pour toutes tes entités
        public DbSet<User> Users => Set<User>();
    }
}
