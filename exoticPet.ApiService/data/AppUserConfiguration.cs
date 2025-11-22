using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace exoticPet.ApiService.Data;

public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Username).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Email).HasMaxLength(200);

        builder.HasData(
            new AppUser { Id = 1, Username = "user1", Email = "user1@example.com" },
            new AppUser { Id = 2, Username = "user2", Email = "user2@example.com" }
        );
    }
}
