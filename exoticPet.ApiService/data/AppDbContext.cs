using Microsoft.EntityFrameworkCore;

namespace exoticPet.ApiService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Animal> Animals => Set<Animal>();
    public DbSet<Environnement> Environnements => Set<Environnement>();
    public DbSet<AppUser> Users => Set<AppUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new AnimalConfiguration().Configure(modelBuilder.Entity<Animal>());
        new EnvironnementConfiguration().Configure(modelBuilder.Entity<Environnement>());
        new AppUserConfiguration().Configure(modelBuilder.Entity<AppUser>());
        base.OnModelCreating(modelBuilder);
    }
}
