using Microsoft.EntityFrameworkCore;

namespace exoticPet.ApiService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Animal> Animals => Set<Animal>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new AnimalConfiguration().Configure(modelBuilder.Entity<Animal>());
        base.OnModelCreating(modelBuilder);
    }
}
