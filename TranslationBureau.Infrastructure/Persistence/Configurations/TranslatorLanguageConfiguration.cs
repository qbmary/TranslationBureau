using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TranslationBureau.Domain.Entities;

namespace TranslationBureau.Infrastructure.Persistence.Configurations;

/// <summary>Конфигурация отображения связи «переводчик — язык» на таблицу.</summary>
public class TranslatorLanguageConfiguration : IEntityTypeConfiguration<TranslatorLanguage>
{
    public void Configure(EntityTypeBuilder<TranslatorLanguage> builder)
    {
        builder.ToTable("TranslatorLanguages");

        // Составной первичный ключ: пара (TranslatorId, LanguageId) уникальна
        builder.HasKey(tl => new { tl.TranslatorId, tl.LanguageId });

        builder.HasOne(tl => tl.Translator)
            .WithMany(t => t.Languages)
            .HasForeignKey(tl => tl.TranslatorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(tl => tl.Language)
            .WithMany(l => l.TranslatorLanguages)
            .HasForeignKey(tl => tl.LanguageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}