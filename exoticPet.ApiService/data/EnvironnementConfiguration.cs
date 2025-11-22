using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace exoticPet.ApiService.Data;

public class EnvironnementConfiguration : IEntityTypeConfiguration<Environnement>
{
    public void Configure(EntityTypeBuilder<Environnement> builder)
    {
        builder.ToTable("Environnements");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Description).HasMaxLength(500);

        builder.HasData(
            new Environnement { Id = 1, Name = "Forêt tropicale", Description = "Climat chaud et humide, végétation dense." },
            new Environnement { Id = 2, Name = "Savane", Description = "Zones herbeuses avec saisons sèches et humides." },
            new Environnement { Id = 3, Name = "Désert", Description = "Climat aride, fortes amplitudes thermiques." }
        );
    }
}
