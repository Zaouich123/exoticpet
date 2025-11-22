using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace exoticPet.ApiService.Data;

public class AnimalConfiguration : IEntityTypeConfiguration<Animal>
{
    public void Configure(EntityTypeBuilder<Animal> builder)
    {
        builder.ToTable("Animals");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Name).IsRequired().HasMaxLength(100);
        builder.Property(a => a.Species).IsRequired().HasMaxLength(100);
        builder.Property(a => a.Specifications).IsRequired().HasMaxLength(2000);
        builder.Property(a => a.Price).HasPrecision(10, 2).IsRequired();

        builder.HasOne(a => a.Environment)
            .WithMany(e => e.Animals)
            .HasForeignKey(a => a.EnvironmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Buyer)
            .WithMany(u => u.Animals)
            .HasForeignKey(a => a.BuyerId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
