using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace exoticPet.ApiService.Data;

public class PurchaseRecordConfiguration : IEntityTypeConfiguration<PurchaseRecord>
{
    public void Configure(EntityTypeBuilder<PurchaseRecord> builder)
    {
        builder.ToTable("Purchases");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.FullName).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Address).IsRequired().HasMaxLength(500);
        builder.Property(p => p.CreatedAt).IsRequired();

        builder.HasOne(p => p.Animal)
            .WithMany()
            .HasForeignKey(p => p.AnimalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Buyer)
            .WithMany()
            .HasForeignKey(p => p.BuyerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => p.AnimalId).IsUnique(); // one purchase per animal
    }
}
