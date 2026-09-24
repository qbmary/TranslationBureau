using TranslationBureau.Domain.Common;
using TranslationBureau.Domain.Exceptions;

namespace TranslationBureau.Domain.Entities;

/// <summary>Переводчик бюро переводов.</summary>
public class Translator : Entity
{
    /// <summary>Конструктор без параметров требуется Entity Framework Core.</summary>
    private Translator()
    {
    }

    private Translator(string fullName, string? phone, string? email,
        string? category, decimal ratePerUnit)
    {
        FullName = fullName;
        Phone = phone;
        Email = email;
        Category = category;
        RatePerUnit = ratePerUnit;
        IsActive = true;
    }

    /// <summary>Рабочие языки переводчика (связь «многие ко многим»).</summary>
    public ICollection<TranslatorLanguage> Languages { get; private set; }
        = new List<TranslatorLanguage>();

    public string FullName { get; private set; } = string.Empty;
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public string? Category { get; private set; }
    public decimal RatePerUnit { get; private set; }
    public bool IsActive { get; private set; }

    /// <summary>Фабричный метод создания переводчика с проверкой правил
    /// предметной области.</summary>
    public static Translator Create(string fullName, string? phone, string? email,
        string? category, decimal ratePerUnit)
    {
        Validate(fullName, ratePerUnit);

        return new Translator(fullName.Trim(), phone, email, category, ratePerUnit);
    }

    /// <summary>Изменяет реквизиты переводчика.</summary>
    public void Update(string fullName, string? phone, string? email,
        string? category, decimal ratePerUnit)
    {
        Validate(fullName, ratePerUnit);

        FullName = fullName.Trim();
        Phone = phone;
        Email = email;
        Category = category;
        RatePerUnit = ratePerUnit;
    }

    /// <summary>Переводит запись в архивное состояние вместо физического удаления.</summary>
    public void Deactivate() => IsActive = false;

    public void Activate() => IsActive = true;

    /// <summary>
    /// Задаёт рабочие языки переводчика. Требуется не менее одного языка.
    /// Существующие связки для переданных языков сохраняются — пересоздаются
    /// только новые, удаляются только лишние.
    /// </summary>
    public void SetLanguages(IEnumerable<Language> languages)
    {
        var list = languages?.ToList() ?? new List<Language>();

        if (list.Count == 0)
            throw new DomainException("Переводчик должен владеть хотя бы одним рабочим языком.");

        if (list.Any(l => l is null))
            throw new DomainException("Список языков содержит пустую ссылку.");

        var targetIds = list.Select(l => l.Id).ToList();

        if (list.Distinct(ReferenceEqualityComparer.Instance).Count() != list.Count)
            throw new DomainException("Список языков содержит дубликаты.");

        // Удаляем связки, которых нет в новом списке
        var toRemove = Languages
            .Where(tl => !targetIds.Contains(tl.LanguageId))
            .ToList();
        foreach (var link in toRemove)
            Languages.Remove(link);

        // Добавляем новые (тех, которых ещё нет)
        var existingIds = Languages.Select(tl => tl.LanguageId).ToHashSet();
        foreach (var language in list)
        {
            if (existingIds.Contains(language.Id))
                continue;

            Languages.Add(new TranslatorLanguage(Id, language.Id)
            {
                Translator = this,
                Language = language
            });
        }
    }

    /// <summary>
    /// Проверяет, владеет ли переводчик обоими языками указанной пары.
    /// </summary>
    public bool KnowsLanguagePair(string codeA, string codeB)
    {
        if (string.IsNullOrWhiteSpace(codeA) || string.IsNullOrWhiteSpace(codeB))
            throw new DomainException("Коды языков пары не могут быть пустыми.");

        var codes = Languages
            .Select(tl => tl.Language.Code)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return codes.Contains(codeA) && codes.Contains(codeB);
    }

    private static void Validate(string fullName, decimal ratePerUnit)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new DomainException("ФИО переводчика не может быть пустым.");
        }

        if (ratePerUnit <= 0)
        {
            throw new DomainException("Ставка переводчика должна быть положительной.");
        }
    }
}