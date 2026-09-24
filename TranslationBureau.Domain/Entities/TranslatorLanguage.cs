namespace TranslationBureau.Domain.Entities;

/// <summary>
/// Связь переводчика и языка (отношение «многие ко многим»).
/// Отдельная сущность требуется потому, что в реляционной базе
/// связь «многие ко многим» реализуется через промежуточную таблицу.
/// </summary>
public class TranslatorLanguage
{
    /// <summary>
    /// Конструктор без параметров требуется Entity Framework Core
    /// для материализации строк из таблицы TranslatorLanguages.
    /// </summary>
    private TranslatorLanguage()
    {
    }

    /// <summary>
    /// Конструктор, доступный только внутри сборки TranslationBureau.Domain.
    /// Внешний код не должен создавать связки напрямую — это делается
    /// исключительно методом Translator.SetLanguages.
    /// </summary>
    internal TranslatorLanguage(int translatorId, int languageId)
    {
        TranslatorId = translatorId;
        LanguageId = languageId;
    }

    /// <summary>Идентификатор переводчика (часть составного первичного ключа).</summary>
    public int TranslatorId { get; private set; }

    /// <summary>Навигационное свойство на переводчика.</summary>
    public Translator Translator { get; internal set; } = null!;

    /// <summary>Идентификатор языка (вторая часть составного первичного ключа).</summary>
    public int LanguageId { get; private set; }

    /// <summary>Навигационное свойство на язык.</summary>
    public Language Language { get; internal set; } = null!;
}