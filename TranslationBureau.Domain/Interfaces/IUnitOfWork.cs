namespace TranslationBureau.Domain.Interfaces;

/// <summary>
/// Контракт единицы работы: сохраняет изменения, накопленные
/// репозиториями в текущем сценарии использования.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}