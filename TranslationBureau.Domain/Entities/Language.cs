using TranslationBureau.Domain.Exceptions;

namespace TranslationBureau.Domain.Entities;

/// <summary>Язык, с которым работает бюро переводов.</summary>
public class Language
{
    /// <summary>Конструктор без параметров требуется Entity Framework Core.</summary>
    private Language()
    {
    }

    private Language(string code, string name, string direction)
    {
        Code = code.Trim();
        Name = name.Trim();
        Direction = direction.Trim();
    }

    public int Id { get; private set; }

    /// <summary>Код языка (ISO 639-1, например "en").</summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>Наименование языка.</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>Направление письма: "ltr" или "rtl".</summary>
    public string Direction { get; private set; } = string.Empty;

    /// <summary>Навигация для связи «многие ко многим».</summary>
    public ICollection<TranslatorLanguage> TranslatorLanguages { get; private set; }
        = new List<TranslatorLanguage>();

    /// <summary>Фабричный метод создания языка с проверкой правил предметной области.</summary>
    public static Language Create(string code, string name, string direction)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Код языка не может быть пустым.");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Наименование языка не может быть пустым.");

        if (direction != "ltr" && direction != "rtl")
            throw new DomainException("Направление письма должно быть \"ltr\" или \"rtl\".");

        return new Language(code, name, direction);
    }

    /// <summary>Изменяет наименование и направление письма.</summary>
    public void Update(string name, string direction)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Наименование языка не может быть пустым.");

        if (direction != "ltr" && direction != "rtl")
            throw new DomainException("Направление письма должно быть \"ltr\" или \"rtl\".");

        Name = name.Trim();
        Direction = direction.Trim();
    }
}