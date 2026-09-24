using TranslationBureau.Domain.Entities;
using TranslationBureau.Domain.Exceptions;

namespace TranslationBureau.Domain.Tests;

public class TranslatorTests
{
    // ================= Create =================

    [Fact]
    public void Create_WithEmptyFullName_ThrowsDomainException()
    {
        // Arrange
        var fullName = "   ";
        var ratePerUnit = 20m;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
            Translator.Create(fullName, null, null, null, ratePerUnit));

        Assert.Contains("ФИО", exception.Message);
    }

    [Fact]
    public void Create_WithZeroRate_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            Translator.Create("Иванов Иван Иванович", null, null, null, 0m));

        Assert.Contains("ставка", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_WithNegativeRate_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            Translator.Create("Иванов Иван Иванович", null, null, null, -5m));

        Assert.Contains("ставка", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_WithValidData_ReturnsActiveTranslator()
    {
        var translator = Translator.Create("Иванов Иван Иванович",
            "+375291234567", "ivanov@example.com", "высшая", 22m);

        Assert.Equal("Иванов Иван Иванович", translator.FullName);
        Assert.Equal(22m, translator.RatePerUnit);
        Assert.True(translator.IsActive);
    }

    // ================= Update =================

    [Fact]
    public void Update_WithEmptyFullName_ThrowsDomainException()
    {
        var translator = Translator.Create("Иванов Иван Иванович",
            null, null, null, 20m);

        Assert.Throws<DomainException>(() =>
            translator.Update("", null, null, null, 25m));
    }

    [Fact]
    public void Update_WithNegativeRate_ThrowsDomainException()
    {
        var translator = Translator.Create("Иванов Иван Иванович",
            null, null, null, 20m);

        Assert.Throws<DomainException>(() =>
            translator.Update("Иванов Иван Иванович", null, null, null, -1m));
    }

    // ================= Deactivate / Activate =================

    [Fact]
    public void Deactivate_SetsIsActiveToFalse()
    {
        var translator = Translator.Create("Иванов Иван Иванович",
            null, null, null, 20m);
        Assert.True(translator.IsActive);

        translator.Deactivate();

        Assert.False(translator.IsActive);
    }

    [Fact]
    public void Activate_SetsIsActiveToTrue()
    {
        var translator = Translator.Create("Иванов Иван Иванович",
            null, null, null, 20m);
        translator.Deactivate();

        translator.Activate();

        Assert.True(translator.IsActive);
    }

    // ================= SetLanguages =================

    [Fact]
    public void SetLanguages_WithEmptyList_ThrowsDomainException()
    {
        var translator = Translator.Create("Иванов Иван Иванович",
            null, null, null, 20m);

        Assert.Throws<DomainException>(() =>
            translator.SetLanguages(new List<Language>()));
    }

    [Fact]
    public void SetLanguages_WithOneLanguage_AddsItToCollection()
    {
        var translator = Translator.Create("Иванов Иван Иванович",
            null, null, null, 20m);
        var english = Language.Create("en", "Английский", "ltr");

        translator.SetLanguages(new[] { english });

        Assert.Single(translator.Languages);
    }

    // ================= KnowsLanguagePair =================

    [Fact]
    public void KnowsLanguagePair_WithBothKnown_ReturnsTrue()
    {
        var translator = Translator.Create("Иванов Иван Иванович",
            null, null, null, 20m);
        var english = Language.Create("en", "Английский", "ltr");
        var german = Language.Create("de", "Немецкий", "ltr");

        translator.SetLanguages(new[] { english, german });

        Assert.True(translator.KnowsLanguagePair("en", "de"));
    }

    [Fact]
    public void KnowsLanguagePair_WithOneUnknown_ReturnsFalse()
    {
        var translator = Translator.Create("Иванов Иван Иванович",
            null, null, null, 20m);
        var english = Language.Create("en", "Английский", "ltr");

        translator.SetLanguages(new[] { english });

        Assert.False(translator.KnowsLanguagePair("en", "fr"));
    }
}