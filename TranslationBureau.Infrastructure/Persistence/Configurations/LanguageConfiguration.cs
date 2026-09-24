using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TranslationBureau.Domain.Entities;

namespace TranslationBureau.Infrastructure.Persistence.Configurations;

/// <summary>Конфигурация отображения сущности «Язык» на таблицу.</summary>
public class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.ToTable("Languages");
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Code)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(l => l.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(l => l.Direction)
            .IsRequired()
            .HasMaxLength(3);

        builder.HasIndex(l => l.Code).IsUnique();
    }
}