using Microsoft.EntityFrameworkCore;
using Task.Models;
using Task.Models.Common;

namespace Task.Contexts
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Slide> Slides { get; set; } = null!; // Decleare Table
        public DbSet<Service> Services { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;

        // Create and Modifie time `ı avtomatik set olunmasını təmin etmək
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Database`dən çəkilən hər hansı bir modeli izləyir. Onun üzərində olunan dəyişiklikkləri sona qədər izləyir və sonda database`ə save edir
            var entries  = ChangeTracker.Entries<BaseEntity>();
            foreach (var item in entries)
            {
                switch (item.State)
                {
                 
                    case EntityState.Unchanged:
                        item.Entity.CreatedAt = DateTime.UtcNow;
                        item.Entity.ModifiedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        item.Entity.ModifiedAt = DateTime.UtcNow;
                        break;
                }
            }
            //EntityState //- Enumdır

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
