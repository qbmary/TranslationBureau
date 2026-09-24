using TranslationBureau.Domain.Entities;

namespace TranslationBureau.Domain.Interfaces;

/// <summary>Контракт доступа к справочнику языков.</summary>
public interface ILanguageRepository
{
    IQueryable<Language> Query();
}