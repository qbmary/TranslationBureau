using Microsoft.EntityFrameworkCore;
using TranslationBureau.Domain.Entities;

namespace TranslationBureau.Infrastructure.Persistence;

/// <summary>Контекст базы данных бюро переводов.</summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Translator> Translators => Set<Translator>();
    public DbSet<Language> Languages => Set<Language>();
    public DbSet<TranslatorLanguage> TranslatorLanguages => Set<TranslatorLanguage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}