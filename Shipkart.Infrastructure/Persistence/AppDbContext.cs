    using Shipkart.Domain.Entities;
    using Microsoft.EntityFrameworkCore;

    namespace Shipkart.Api
    {
        public class AppDbContext : DbContext
        {
            public DbSet<User> Users => Set<User>();

            public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
            { }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            }

        }
    }
