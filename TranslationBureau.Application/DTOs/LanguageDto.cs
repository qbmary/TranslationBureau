namespace TranslationBureau.Application.DTOs;

/// <summary>Объект передачи сведений о языке в слой представления.</summary>
public class LanguageDto
{
    public int Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Direction { get; init; } = string.Empty;
}