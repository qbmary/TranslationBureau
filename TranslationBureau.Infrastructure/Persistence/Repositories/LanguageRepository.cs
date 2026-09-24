using Microsoft.EntityFrameworkCore;
using TranslationBureau.Domain.Entities;
using TranslationBureau.Domain.Interfaces;

namespace TranslationBureau.Infrastructure.Persistence.Repositories;

public class LanguageRepository : ILanguageRepository
{
    private readonly ApplicationDbContext _context;

    public LanguageRepository(ApplicationDbContext context)
        => _context = context;

    public IQueryable<Language> Query()
        => _context.Languages;
}